using LoanManagementSystem.GetModel;
using LoanManagementSystem.Models;
using LoanManagementSystem.Repositories.RepositoryInterface;
using LoanManagementSystem.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LMSUnitTest
{
    public class LMSUnitTest
    {
        [Fact]
        public void Test_Valid_For_WithdrawAmount()
        {
            // Arrange
            TelerivetService telerivetService = new TelerivetService();
            // Act
            bool checkData = telerivetService.CheckifRequestDataIsForWithdrawAmount("4", "1234");
            // Assert
            Assert.True(checkData);
        }

        [Fact]
        public void Test_InValid_For_WithdrawAmount()
        {
            // Arrange
            TelerivetService telerivetService = new TelerivetService();
            // Act
            bool checkData = telerivetService.CheckifRequestDataIsForWithdrawAmount("4", "string");
            // Assert
            Assert.False(checkData);
        }

        [Fact]
        public void Test_For_GetUnApprovedTransactions()
        {
            // Arrange
            var mockTransactionRepository = new Mock<ITransactionRepository>();
            var mockUserRepository = new Mock<IUserRepository>();
            mockTransactionRepository.Setup(p => p.GetUnApprovedTransactions()).Returns(new List<GetTransaction>
            {
                new GetTransaction {
                    TransactionId = Guid.NewGuid(),
                    SenderMobileNumber = "5550123",
                    ReceiverMobileNumber = "5550124",
                    Amount = 100,
                    Name = "Jack",
                    ApproveStatus = 0,
                    ProceededAt = DateTime.UtcNow,
                    CompletedAt = DateTime.UtcNow
                },
                new GetTransaction {
                    TransactionId = Guid.NewGuid(),
                    SenderMobileNumber = "5550125",
                    ReceiverMobileNumber = "5550126",
                    Amount = 200,
                    Name = "Mike",
                    ApproveStatus = 0,
                    ProceededAt = DateTime.UtcNow,
                    CompletedAt = DateTime.UtcNow
                }

            });

            // Act
            TransactionService transactionService = new TransactionService(mockTransactionRepository.Object, mockUserRepository.Object);

            // Assert
            Assert.NotNull(transactionService.GetUnApprovedTransactions());
            Assert.Equal(2, transactionService.GetUnApprovedTransactions().Count);
        }

        [Fact]
        public async Task Test_ApproveStatus()
        {
            // Arrange 
            var mockTransactionRepository = new Mock<ITransactionRepository>();
            var mockUserRepository = new Mock<IUserRepository>();

            Guid id = Guid.NewGuid();
            string phoneNumber = "123456";

            mockTransactionRepository.Setup(p => p.ApproveStatusDataManager(id, phoneNumber)).Returns(Task.FromResult(true));

            // Act
            TransactionService transactionService = new TransactionService(mockTransactionRepository.Object, mockUserRepository.Object);

            // Assert
            Assert.True(await transactionService.ApproveStatus(id, phoneNumber));
        }

        [Fact]
        public async Task Test_RejectStatus()
        {
            // Arrange 
            var mockTransactionRepository = new Mock<ITransactionRepository>();
            var mockUserRepository = new Mock<IUserRepository>();

            Guid id = Guid.NewGuid();
            string phoneNumber = "123456";

            mockTransactionRepository.Setup(p => p.RejectStatusDataManager(id, phoneNumber)).Returns(Task.FromResult(true));

            // Act
            TransactionService transactionService = new TransactionService(mockTransactionRepository.Object, mockUserRepository.Object);

            // Assert
            Assert.True(await transactionService.RejectStatus(id, phoneNumber));
        }

        [Fact]
        public void Test_WithdrawAmount()
        {
            // Arrange
            var mockTransactionRepository = new Mock<ITransactionRepository>();
            var mockUserRepository = new Mock<IUserRepository>();

            // Act
            TransactionService transactionService = new TransactionService(mockTransactionRepository.Object, mockUserRepository.Object);
            decimal amount = 100;
            string pincode = "1234";
            string fromNumber = "654321";
            mockUserRepository.Setup(p => p.CheckPhonenumberAlreadyExists(fromNumber)).Returns(true);
            mockUserRepository.Setup(p => p.GetAccountCredentials(fromNumber)).Returns(new AccountCredentials
            {
                Id = It.IsAny<Guid>(),
                PhoneNumber = "654321",
                Balance = 10000,
                UserId = It.IsAny<Guid>()
            });
            mockUserRepository.Setup(p => p.CheckPincode(It.IsAny<Guid>(), It.IsAny<int>())).Returns(true);
            mockTransactionRepository.Setup(p => p.BalanceUpdateUserDetails(new BalanceUpdateUserDetails
            {
                Id = It.IsAny<Guid>(),
                UserId = It.IsAny<Guid>(),
                Amount = 10000,
                ProceededAt = It.IsAny<DateTime>(),
                Action = It.IsAny<string>(),
                Status = It.IsAny<int>(),
                ActionDate = It.IsAny<DateTime>()
            }));

            // Assert
            Assert.True(transactionService.WithDrawAmount(amount, pincode, fromNumber));

        }

        [Fact]
        public void Test_CreateTransaction()
        {
            // Arrange
            string toNumber = "12345";
            string fromNumber = "54321";
            var mockTransactionRepository = new Mock<ITransactionRepository>();
            var mockUserRepository = new Mock<IUserRepository>();

            mockUserRepository.Setup(p => p.CheckPhonenumberAlreadyExists(fromNumber)).Returns(true);
            mockUserRepository.Setup(p => p.CheckPhonenumberAlreadyExists(toNumber)).Returns(true);

            mockUserRepository.Setup(p => p.GetAccountCredentials(fromNumber)).Returns(new AccountCredentials
            {
                Id = It.IsAny<Guid>(),
                PhoneNumber = "54321",
                Balance = 10000,
                UserId = It.IsAny<Guid>()
            });
            mockUserRepository.Setup(p => p.CheckPincode(It.IsAny<Guid>(), It.IsAny<int>())).Returns(true);
            mockUserRepository.Setup(p => p.CheckPhonenumberAlreadyExists(It.IsAny<string>())).Returns(true);
            mockUserRepository.Setup(p => p.ValidateTransactionByUserType(It.IsAny<string>())).Returns(true);


            mockTransactionRepository.Setup(p => p.AddTransaction(new Transaction
            {
                Id = Guid.NewGuid(),
                SenderMobileNumber = It.IsAny<string>(),
                ReceiverMobileNumber = It.IsAny<string>(),
                Amount = It.IsAny<decimal>(),
                ApproveStatus = It.IsAny<int>(),
                ProceededAt = It.IsAny<DateTime>()
            }));


            // Act
            TransactionService transactionService = new TransactionService(mockTransactionRepository.Object, mockUserRepository.Object);




            // Assert
            Assert.True(transactionService.CreateTransaction(toNumber, 100, "1234", fromNumber));
        }

        [Fact]
        public void Test_RegisterUser()
        {
            // Arrange
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(p => p.CheckUserTypeExists(1)).Returns(true);
            mockUserRepository.Setup(p => p.CheckPhonenumberAlreadyExists(It.IsAny<string>())).Returns(false);
            mockUserRepository.Setup(p => p.AddUser(new User
            {
                Id = Guid.NewGuid(),
                UserTypeId = It.IsAny<int>(),
                Name = It.IsAny<string>()
            }));
            mockUserRepository.Setup(p => p.AddPincodeCredentials(new PincodeCredentials
            {
                Id = It.IsAny<Guid>(),
                UserId = It.IsAny<Guid>(),
                PinCode = 1234
            }));
            mockUserRepository.Setup(p => p.AddAccountCredentials(new AccountCredentials
            {
                Id = It.IsAny<Guid>(),
                UserId = It.IsAny<Guid>(),
                PhoneNumber = It.IsAny<string>()
            }));

            // Act
            UserService userService = new UserService(mockUserRepository.Object);
            bool RegisterUser = userService.RegisterUser("1", It.IsAny<string>(), "1234", It.IsAny<string>());

            // Assert
            Assert.True(RegisterUser);
          
        }
    }
}
