// using Application.Repositories;
// using Application.Users.Commands;
// using NSubstitute;
// using NSubstitute.ReturnsExtensions;
//
// namespace Domain.UnitTests.UseCases.User.Commands.ConfigureUserTenantTests;
//
// [TestClass]
// public class Validator
// {
//     private readonly ConfigureUserTenant.Validator _validator;
//
//     public Validator()
//     {
//         var tenantRepository = Substitute.For<ITenantRepository>();
//
//         tenantRepository.GetTenantById(default!, default)
//             .ReturnsNullForAnyArgs();
//
//         var userRepository = Substitute.For<IUserRepository>();
//
//         userRepository.GetByIdAsync(default!, default)
//             .ReturnsNullForAnyArgs();
//
//         _validator = new ConfigureUserTenant.Validator(tenantRepository, userRepository);
//     }
//
//     [TestMethod]
//     public async Task AllValidations()
//     {
//         // Arrange
//         var command = new ConfigureUserTenant.Command(new("", ""), 0);
//
//         // Act
//         var validationResult = await _validator.ValidateAsync(command);
//
//         // Assert
//         Assert.IsFalse(validationResult.IsValid);
//
//         var expectedErrorList = new List<string>
//         {
//             "'Token Value' must not be empty.",
//             "'Token Refresh Value' must not be empty.",
//             "'Tenant Id' must be greater than '0'.",
//             "Tenant with id '0' not found.",
//             "User token is invalid.",
//         };
//
//         validationResult.Errors
//             .Select(e => e.ErrorMessage)
//             .Should()
//             .Equal(expectedErrorList);
//     }
// }