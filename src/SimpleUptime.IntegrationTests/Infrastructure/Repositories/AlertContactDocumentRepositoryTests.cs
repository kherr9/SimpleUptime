using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using SimpleUptime.Domain.Models;
using SimpleUptime.Infrastructure.Repositories;
using SimpleUptime.IntegrationTests.Fixtures;
using Xunit;

namespace SimpleUptime.IntegrationTests.Infrastructure.Repositories
{
    public class AlertContactDocumentRepositoryTests : IClassFixture<DocumentDbFixture>, IDisposable
    {
        private readonly DocumentDbFixture _fixture;
        private readonly AlertContactDocumentRepository _repository;

        public AlertContactDocumentRepositoryTests(DocumentDbFixture fixture)
        {
            _fixture = fixture;
            _repository = new AlertContactDocumentRepository(fixture.DocumentClient, DatabaseConfigurations.Create());
        }

        public void Dispose()
        {
            _fixture.Reset();
        }

        #region Create

        [Theory]
        [MemberData(nameof(ContactAlertInstances))]
        public async Task CreateAlertContacts(IAlertContact entity)
        {
            // Act
            await _repository.CreateAsync(entity);

            // Assert
            var readEntity = await _repository.GetAsync(entity.Id);
            Assert.Equal(entity.Id, readEntity.Id);
        }

        [Fact]
        public async Task CreateThrowsExceptionWhenIdExists()
        {
            // Arrange
            var existingEntityId = (await GenerateAndPersistEmailAlertContactAsync()).Id;
            var entity = GenerateEmailAlertContact();
            entity.Id = existingEntityId;

            // Act
            var ex = await Assert.ThrowsAsync<DocumentClientException>(() => _repository.CreateAsync(entity));

            // Assert
            Assert.Equal("Conflict", ex.Error.Code);
            Assert.True(ex.Message.Contains("Resource with specified id or name already exists"), ex.Message);
        }

        [Fact]
        public async Task CreateThrowsExceptionWhenEntityIsNull()
        {
            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => _repository.CreateAsync(null));

            // Assert
            Assert.Equal("alertContact", exception.ParamName);
        }

        #endregion

        #region GetById

        [Theory]
        [MemberData(nameof(ContactAlertInstances))]
        public async Task GetByIdAllTypes(IAlertContact entity)
        {
            // Arrange
            await _repository.CreateAsync(entity);

            // Act
            var readEntity = await _repository.GetAsync(entity.Id);

            // Assert
            Assert.Equal(entity.Id, readEntity.Id);
            Assert.Equal(entity.GetType(), readEntity.GetType());
        }

        [Fact]
        public async Task GetByIdReturnsNullWhenNotExists()
        {
            // Arrange
            var entityId = AlertContactId.Create();

            // Act
            var readEntity = await _repository.GetAsync(entityId);

            // Assert
            Assert.Null(readEntity);
        }

        #endregion

        #region GetByIdGeneric

        [Theory]
        [MemberData(nameof(ContactAlertInstances))]
        public async Task GetByIdGenericAllTypes(IAlertContact entity)
        {
            // Arrange
            await _repository.CreateAsync(entity);

            var method = _repository.GetType().GetMethods()
                .Single(m => m.Name == "GetAsync" && m.IsGenericMethod);
            var generic = method.MakeGenericMethod(entity.GetType());

            // Act
            var readEntity = (IAlertContact)(await (dynamic)generic.Invoke(_repository, new object[] { entity.Id }));

            // Assert
            Assert.Equal(entity.Id, readEntity.Id);
            Assert.Equal(entity.GetType(), readEntity.GetType());
        }

        [Fact]
        public async Task GetByIdGenericReturnsNullWhenNotExists()
        {
            // Arrange
            var entityId = AlertContactId.Create();

            // Act
            var readEntity = await _repository.GetAsync<EmailAlertContact>(entityId);

            // Assert
            Assert.Null(readEntity);
        }

        [Fact]
        public async Task GetByIdGenericThrowExceptionWhenTypeDoesNotMatch()
        {
            // Arrange
            var emailAlertContact = await GenerateAndPersistEmailAlertContactAsync();

            // Act
            await Assert.ThrowsAsync<InvalidCastException>(() => _repository.GetAsync<SlackAlertContact>(emailAlertContact.Id));
        }

        #endregion

        #region GetAll

        [Fact]
        public async Task GetAllWhenEmpty()
        {
            // Act
            var result = await _repository.GetAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllWhenEmailAndSlack()
        {
            // Arrange
            var emailAlert = await GenerateAndPersistEmailAlertContactAsync();
            var slackAlert = await GenerateAndPersistSlackAlertContactAsync();

            // Act
            var result = (await _repository.GetAsync()).ToArray();

            // Assert
            Assert.Equal(2, result.Length);

            var readEmailAlert = result.Single(x => x.Id == emailAlert.Id);
            Assert.IsType<EmailAlertContact>(readEmailAlert);

            var readSlackAlert = result.Single(x => x.Id == slackAlert.Id);
            Assert.IsType<SlackAlertContact>(readSlackAlert);
        }

        #endregion

        public static IEnumerable<object[]> ContactAlertInstances()
        {
            yield return new object[] { GenerateEmailAlertContact() };
            yield return new object[] { GenerateSlackAlertContact() };
        }

        private static EmailAlertContact GenerateEmailAlertContact()
        {
            return new EmailAlertContact()
            {
                Id = AlertContactId.Create(),
                Email = "foo@example.com"
            };
        }

        private static SlackAlertContact GenerateSlackAlertContact()
        {
            return new SlackAlertContact()
            {
                Id = AlertContactId.Create(),
                WebHookUrl = new Uri("http://slack.example.com/hook")
            };
        }

        private async Task<EmailAlertContact> GenerateAndPersistEmailAlertContactAsync()
        {
            var entity = GenerateEmailAlertContact();

            await _repository.CreateAsync(entity);

            return entity;
        }

        private async Task<SlackAlertContact> GenerateAndPersistSlackAlertContactAsync()
        {
            var entity = GenerateSlackAlertContact();

            await _repository.CreateAsync(entity);

            return entity;
        }
    }
}
