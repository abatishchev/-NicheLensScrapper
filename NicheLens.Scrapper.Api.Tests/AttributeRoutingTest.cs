using System;
using System.Linq;
using System.Reflection;
using System.Web.Http;

using FluentAssertions;
using Xunit;

namespace NicheLens.Scrapper.Api.Tests
{
	public class AttributeRoutingTest
	{
		[Fact]
		public void Every_Method_Of_ApiController_Returning_IHttpActionResult_Should_Be_Decorated_With_RouteAttribute()
		{
			// Arrange
			var assembly = Assembly.Load("NicheLens.Scrapper.Api");

			// Act
			var attributes = from t in assembly.GetTypes()
							 where typeof(ApiController).IsAssignableFrom(t)
							 from m in t.GetMethods()
							 where IsTypeOf(m.ReturnType, typeof(IHttpActionResult))
							 select m.GetCustomAttributes<RouteAttribute>().FirstOrDefault();

			// Assert
			attributes.Should().NotContainNulls();
		}

		private static bool IsTypeOf(Type returnType, Type targetType)
		{
			return returnType.IsGenericType ? returnType.GenericTypeArguments[0] == targetType : returnType == targetType;
		}
	}
}