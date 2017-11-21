// ReSharper disable NonReadonlyMemberInGetHashCode

namespace SimpleUptime.IntegrationTests.WebApi.Controllers.Client
{
    public class HttpResponseDto
    {
        public int StatusCode { get; set; }

        public override bool Equals(object obj)
        {
            return obj is HttpResponseDto dto &&
                   StatusCode == dto.StatusCode;
        }

        public override int GetHashCode()
        {
            return -763886418 + StatusCode.GetHashCode();
        }
    }
}