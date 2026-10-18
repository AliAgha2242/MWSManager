using Application.ServiceContract;
using Infrastructure.DataBaseRepository;
using Model.ApplicationModels;
using Model.PublicModels;
using RabbitMq_Infrastructure.SqlConnectionManagerGroup;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ServiceImplement
{
    public class RequestHubService : IRequestHubService
    {
        private readonly SqlConnectionManager connectionManager;
        private readonly IFacadDesign facadDesign;
        public RequestHubService(SqlConnectionManager connectionManager, IFacadDesign facad)
        {
            this.connectionManager = connectionManager;
            facadDesign = facad;
        }
        public async Task<OperationResult> CreateAsync(RabbitConsumerRequestHub requestHub)
        {
            try
            {
                await facadDesign.repository.CreateAsync(requestHub);
                await facadDesign.SaveChangeAsync();
                return new OperationResult();
            }
            catch (Exception ex)
            {
                return new OperationResult(false, ex.Message);
            }

        }

        public async Task<OperationResult> DeleteAsync(RabbitConsumerRequestHub requestHub)
        {
            // Sql sql = new Sql() { Text = "sp_RabbitConsumerRequestHub_Delete", commandType = System.Data.CommandType.StoredProcedure, Parameters = new { requestHub.Id } };
            try
            {
                //   await connectionManager.ExecuteAsync(sql);
                await facadDesign.repository.DeleteAsync(requestHub);
                await facadDesign.SaveChangeAsync();
                return new OperationResult();
            }
            catch (Exception ex)
            {
                return new OperationResult(false, ex.Message);
            }

        }

        public async Task<OperationResult> DeleteAsync(int Id)
        {
            //Sql sql = new Sql() { Text = "sp_RabbitConsumerRequestHub_Delete", commandType = System.Data.CommandType.StoredProcedure, Parameters = new { Id } };
            try
            {
                //await connectionManager.ExecuteAsync(sql);
                await facadDesign.repository.DeleteAsync(await facadDesign.repository.GetAsync<RabbitConsumerRequestHub>(p => p.Id == Id));
                await facadDesign.SaveChangeAsync();
                return new OperationResult();
            }
            catch (Exception ex)
            {
                return new OperationResult(false, ex.Message);
            }
        }

        public OperationResult<IEnumerable<RabbitConsumerRequestHub>> ReadAll()
        {
            //Sql sql = new Sql()
            //{
            //    Text = $"Select * from RabbitConsumerRequestHub",
            //    commandType = System.Data.CommandType.Text
            //};

            try
            {
                //IEnumerable<RabbitConsumerRequestHub> data = connectionManager.Query<RabbitConsumerRequestHub>(sql);
                List<RabbitConsumerRequestHub> data = facadDesign.repository.GetAll<RabbitConsumerRequestHub>();
                return new OperationResult<IEnumerable<RabbitConsumerRequestHub>>(data, true, null);
            }
            catch (Exception ex)
            {

                return new OperationResult<IEnumerable<RabbitConsumerRequestHub>>(null, false, ex.Message);
            }
        }

        public async Task<OperationResult<IEnumerable<RabbitConsumerRequestHub>>> ReadAllAsync()
        {

            

            try
            {
                List<RabbitConsumerRequestHub> data = await facadDesign.repository.GetAllAsync<RabbitConsumerRequestHub>();
                return new OperationResult<IEnumerable<RabbitConsumerRequestHub>>(data, true, null);
            }
            catch (Exception ex)
            {

                return new OperationResult<IEnumerable<RabbitConsumerRequestHub>>(false, ex.Message);
            }
        }

        public async Task<OperationResult<IEnumerable<RabbitConsumerRequestHub>>> ReadAllAsync(Func<RabbitConsumerRequestHub, bool> func)
        {
            try
            {
                //IEnumerable<RabbitConsumerRequestHub> data = connectionManager.Query<RabbitConsumerRequestHub>(sql);
                List<RabbitConsumerRequestHub> data = await facadDesign.repository.GetAllAsync(func);
                return new OperationResult<IEnumerable<RabbitConsumerRequestHub>>(data, true, null);
            }
            catch (Exception ex)
            {

                return new OperationResult<IEnumerable<RabbitConsumerRequestHub>>(false, ex.Message);
            }
        }

        public async Task<OperationResult<RabbitConsumerRequestHub>> ReadAsync(int Id)
        {
            //Sql sql = new Sql()
            //{
            //    Text = $"Select top 1 * from RabbitConsumerRequestHub where id  = {Id}",
            //    commandType = System.Data.CommandType.Text
            //};

            try
            {
                RabbitConsumerRequestHub? data = await facadDesign.repository.GetAsync<RabbitConsumerRequestHub>(p => p.Id == Id);
                return new OperationResult<RabbitConsumerRequestHub>(data, true);
            }
            catch (Exception ex)
            {

                return new OperationResult<RabbitConsumerRequestHub>(false,ex.Message);
            }
        }

        public async Task<OperationResult> UpdateAsync(RabbitConsumerRequestHub requestHub)
        {
            //Sql sql = new Sql()
            //{
            //    Text = "sp_RabbitConsumerRequestHub_Update",
            //    commandType = System.Data.CommandType.StoredProcedure,
            //    Parameters = new object[] { requestHub.Id, requestHub.RequestName, requestHub.IsTransactional, requestHub.RoutingKey }
            //};
            try
            {
                facadDesign.repository.Update(requestHub);
                await facadDesign.SaveChangeAsync();
                return new OperationResult();
            }
            catch (Exception ex)
            {
                return new OperationResult(false, ex.Message);
            }

        }


    }
}
