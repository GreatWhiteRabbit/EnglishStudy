using Microsoft.AspNetCore.Authorization;

namespace EnglishStudy.Authorization {
    public class StatusAuthorizationHandler : AuthorizationHandler<StatusAuthorizationRequirement> {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, StatusAuthorizationRequirement requirement) {

            /*var status = context.User.Claims.Where(item => item.Type == "Status").Select(item => item.Value).ToList();
            if (status.Any(item => item.StartsWith(requirement.Name))) {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;*/

            // 获取cliams中的type为status的值
            var status = context.User.Claims
                .Where(item => item.Type == "Status")
                .Select(item => item.Value)
                .SingleOrDefault();
            // 将requirement.Name根据","分割成数组
            var nameList = requirement.Name.Split(",").ToList();
            if (status == null) return Task.CompletedTask;

            foreach ( var item in nameList ) {
                if(item.Equals(status)) {
                    context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;
        }
       
    }
}
