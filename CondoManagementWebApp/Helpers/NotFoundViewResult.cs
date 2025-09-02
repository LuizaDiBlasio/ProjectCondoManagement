using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CondoManagementWebApp.Helpers
{
    public class NotFoundViewResult : ViewResult
    {
        /// <summary>
        /// Initializes a new instance of the "NotFoundViewResult" class with the specified view name.
        /// The HTTP status code for the response will be set to 404 (Not Found).
        /// </summary>
        /// <param name="viewName">The name of the view to render.</param>
        public NotFoundViewResult(string viewName)
        {
            ViewName = viewName;
            StatusCode = (int)HttpStatusCode.NotFound;
        }
    }
}
