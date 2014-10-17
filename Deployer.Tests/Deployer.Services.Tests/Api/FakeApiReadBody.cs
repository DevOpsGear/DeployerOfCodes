using System;
using Deployer.Services.Api.Interfaces;

namespace Deployer.Tests.Api
{
    internal class FakeApiReadBody : IApiReadBody
    {
        private byte[] _data;
        private bool _first;

        public void SetUp(byte[] data)
        {
            _data = data;
            _first = true;
        }

        public int ReadBytes(byte[] buffer)
        {
            if(_first)
            {
                _first = false;
                Array.Copy(_data, 0, buffer, 0, _data.Length);
                return _data.Length;
            }
            return 0;
        }
    }
}