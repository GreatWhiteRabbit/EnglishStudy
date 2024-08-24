using Microsoft.AspNetCore.Authorization;

namespace EnglishStudy.Authorization {
    public class StatusAuthorizationRequirement : IAuthorizationRequirement {
        public string Name { get; set; }

        public StatusAuthorizationRequirement(string name) {
            Name = name;
        }
    }
}
