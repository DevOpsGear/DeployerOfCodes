using System;
using Deployer.Services.Api.Interfaces;

namespace Deployer.Tests.Api
{
    internal class FakeApiBody : IApiReadBody
    {
        private int _callIndex;
        private readonly byte[] _crudOne;
        private readonly byte[] _crudTwo;

        public FakeApiBody()
        {
            _callIndex = 0;
            _crudOne = new byte[] {0x00, 0x01, 0x02, 0x03, 0x04};
            _crudTwo = new byte[128];
            for(var idx = 0; idx < 128; idx++)
            {
                _crudTwo[idx] = (byte) (idx + 5);
            }
        }

        public int ReadBytes(byte[] buffer)
        {
            _callIndex++;
            switch(_callIndex)
            {
                case 1:
                    Array.Copy(_crudOne, 0, buffer, 0, _crudOne.Length);
                    return _crudOne.Length;
                case 2:
                    Array.Copy(_crudTwo, 0, buffer, 0, _crudTwo.Length);
                    return _crudTwo.Length;
                default:
                    return 0;
            }
        }
    }
}