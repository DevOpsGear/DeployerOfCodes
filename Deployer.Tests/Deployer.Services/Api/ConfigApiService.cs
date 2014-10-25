using Deployer.Services.Api.Interfaces;
using Deployer.Services.Builders;
using Deployer.Services.Config;
using Deployer.Services.Config.Interfaces;
using Deployer.Services.Models;
using Json.NETMF;
using NeonMika.Interfaces;
using System;
using System.Collections;
using System.Text;
using NeonMika.Util;

namespace Deployer.Services.Api
{
    public class ConfigApiService : IApiService
    {
        private readonly IConfigurationService _configurationService;
        private readonly IGarbage _garbage;
        private const int BufferSize = 1024;

        public ConfigApiService(IConfigurationService configurationService, IGarbage garbage)
        {
            _configurationService = configurationService;
            _garbage = garbage;
        }

        public bool CanRespond(ApiRequest request)
        {
            return request.Url.StartsWith("projects");
        }

        public bool SendResponse(ApiRequest request)
        {
            _garbage.Collect();
            try
            {
                var url = new UrlSplitter(request.Url);
                HandleStuff(request, url);
            }
            catch (Exception ex)
            {
                request.Client.Send500_Failure(ex.ToString());
            }
            _garbage.Collect();
            return true;
        }

        private void HandleStuff(ApiRequest request, UrlSplitter url)
        {
            if (url.Endpoint == "projects")
            {
                if (url.Id == "")
                {
                    HandleDefault(request);
                    return;
                }

                if (url.Option == "")
                {
                    HandleProject(url.Id, request);
                    return;
                }

                if (url.Option == "build" && url.Moar == "")
                {
                    HandleBuild(url.Id, request);
                    return;
                }
            }
            request.Client.Send404_NotFound();
        }

        private void HandleDefault(ApiRequest request)
        {
            if (request.HttpMethod == "GET")
            {
                var projects = _configurationService.GetProjects();
                var bytes = ConfigHashifier.Bytify(projects);
                request.Client.Send200_OK("application/json", bytes.Length);
                request.Client.Send(bytes, bytes.Length);
                return;
            }

            if (request.HttpMethod == "PUT")
            {
                var proj = new ProjectModel();
                UnpackProject(request, proj);
                proj.Slug = "";
                _configurationService.SaveProject(proj);
                request.Client.Send200_OK("application/json");
                return;
            }

            request.Client.Send405_MethodNotAllowed();
        }

        private void HandleProject(string slug, ApiRequest request)
        {
            if (request.HttpMethod == "GET")
            {
                GetOneProject(slug, request);
                return;
            }

            if (request.HttpMethod == "PUT")
            {
                PutOneProject(slug, request);
                return;
            }

            if (request.HttpMethod == "DELETE")
            {
                DeleteOneProject(slug, request);
                return;
            }

            request.Client.Send405_MethodNotAllowed();
        }

        private void GetOneProject(string slug, ApiRequest request)
        {
            try
            {
                var proj = _configurationService.GetProject(slug);
                var bytes = ConfigHashifier.Bytify(proj);
                request.Client.Send200_OK("application/json", bytes.Length);
                request.Client.Send(bytes, bytes.Length);
            }
            catch (ProjectDoesNotExistException)
            {
                request.Client.Send404_NotFound();
            }
        }

        private void PutOneProject(string slug, ApiRequest request)
        {
            try
            {
                var proj = _configurationService.GetProject(slug);
                UnpackProject(request, proj);
                _configurationService.SaveProject(proj);
                request.Client.Send200_OK("application/json");
            }
            catch (ProjectDoesNotExistException)
            {
                request.Client.Send404_NotFound();
            }
            catch (Exception)
            {
                request.Client.Send400_BadRequest();
            }
        }

        private static void UnpackProject(ApiRequest request, ProjectModel proj)
        {
            var buffer = new byte[BufferSize];
            var countBytes = ShortBodyReader.ReadBody(request.Body, buffer);
            var chars = Encoding.UTF8.GetChars(buffer, 0, countBytes);
            var json = new string(chars);
            var project = JsonSerializer.DeserializeString(json) as Hashtable;
            if (project == null) return;

            proj.Title = project["title"] as string;
            proj.Subtitle = project["subtitle"] as string;
            proj.Rank = Int32.Parse(project["rank"].ToString());
            proj.Provider = (BuildServiceProvider) Int32.Parse(project["provider"].ToString());
        }

        private void DeleteOneProject(string slug, ApiRequest request)
        {
            try
            {
                _configurationService.DeleteProject(slug);
                request.Client.Send200_OK("application/json");
            }
            catch (ProjectDoesNotExistException)
            {
                request.Client.Send404_NotFound();
            }
            catch (Exception)
            {
                request.Client.Send400_BadRequest();
            }
        }

        private void HandleBuild(string slug, ApiRequest request)
        {
            if (request.HttpMethod == "GET")
            {
                GetOneBuid(slug, request);
                return;
            }

            if (request.HttpMethod == "PUT")
            {
                PutOneBuild(slug, request);
                return;
            }
            request.Client.Send405_MethodNotAllowed();
        }

        private void GetOneBuid(string slug, ApiRequest request)
        {
            var build = _configurationService.GetBuildParams(slug);
            var json = JsonSerializer.SerializeObject(build);
            var bytes = Encoding.UTF8.GetBytes(json);
            request.Client.Send200_OK("application/json", bytes.Length);
            request.Client.Send(bytes, bytes.Length);
        }

        private void PutOneBuild(string slug, ApiRequest request)
        {
            var buffer = new byte[BufferSize];
            var countBytes = ShortBodyReader.ReadBody(request.Body, buffer);
            var chars = Encoding.UTF8.GetChars(buffer, 0, countBytes);
            var json = new string(chars);
            var build = JsonSerializer.DeserializeString(json) as Hashtable;
            _configurationService.SaveBuildParams(slug, build);
            request.Client.Send200_OK("application/json");
        }
    }
}