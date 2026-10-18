using Application.ServiceContract;
using Model.ApplicationModels;
using Model.PublicModels;
using RabbitMq_Infrastructure.SqlConnectionManagerGroup;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace Application.ServiceImplement
{
    public class RabbitTaskService : IRabbitTaskService
    {
        private readonly IFacadDesign facad;

        //private readonly SqlConnectionManager connectionManager;

        public RabbitTaskService(IFacadDesign facad)
        {
            this.facad = facad;
            //this.connectionManager = connectionManager;
        }

        public async Task<OperationResult> CreateAsync(RabbitTasks taskModels)
        {
            //Sql sql = new Sql()
            //{
            //    Text = "Sp_RabbitTasks_Insert",
            //    commandType = System.Data.CommandType.StoredProcedure,
            //    Parameters = new {taskModels.TaskName,taskModels.ProccessorName,taskModels.IntervalHour,taskModels.IntervalMinute,
            //        taskModels.FromDateTime,taskModels.ToDateTime,taskModels.RoutingKey },
            //    transaction = null
            //};
            try
            {
                await facad.repository.CreateAsync(taskModels);
                await facad.SaveChangeAsync();
                return new OperationResult();
            }
            catch (Exception ex)
            {
                return new OperationResult(false, ex.Message);
            }
        }

        public async Task<OperationResult> DeleteAsync(RabbitTasks taskModels)
        {
            //Sql sql = new Sql() { Text = "sp_RabbitTasks_Delete", commandType = System.Data.CommandType.StoredProcedure, Parameters = new { taskModels.Id } };
            try
            {
                await facad.repository.DeleteAsync(taskModels);
                await facad.SaveChangeAsync();
                return new OperationResult();
            }
            catch (Exception ex)
            {
                return new OperationResult(false, ex.Message);
            }
        }

        public async Task<OperationResult> DeleteAsync(int Id)
        {
            //Sql sql = new Sql() { Text = "sp_RabbitTasks_Delete", commandType = System.Data.CommandType.StoredProcedure, Parameters = new { Id } };
            try
            {
                await facad.repository.DeleteAsync<RabbitTasks>(p=>p.Id == Id);
                await facad.SaveChangeAsync();
                return new OperationResult();
            }
            catch (Exception ex)
            {
                return new OperationResult(false, ex.Message);
            }
        }

        public OperationResult<IEnumerable<RabbitTasks>> ReadAll(params Expression<Func<RabbitTasks, object>>[] includes )
        {


            try
            {
                IEnumerable<RabbitTasks> data = facad.repository.GetAll<RabbitTasks>(includes);
                return new OperationResult<IEnumerable<RabbitTasks>>(data,true,null);
            }
            catch (Exception ex)
            {
                return new OperationResult<IEnumerable<RabbitTasks>>(null, false, ex.Message);
            }
        }

        public async Task<OperationResult<IEnumerable<RabbitTasks>>> ReadAllAsync()
        {
            //Sql sql = new Sql()
            //{
            //    Text = $"Select * from RabbitTasks",
            //    commandType = System.Data.CommandType.Text
            //};

            try
            {
                IEnumerable<RabbitTasks> data = await facad.repository.GetAllAsync<RabbitTasks>();
                return new OperationResult<IEnumerable<RabbitTasks>>(data,true);
            }
            catch (Exception ex)
            {

                return new OperationResult<IEnumerable<RabbitTasks>>(false, ex.Message);
            }
        }

        public async Task<OperationResult<IEnumerable<RabbitTasks>>> ReadAllAsync(params Expression<Func<RabbitTasks, object>>[] includes)
        {
            try
            {
                IEnumerable<RabbitTasks> data = await facad.repository.GetAllAsync<RabbitTasks>(includes);
                return new OperationResult<IEnumerable<RabbitTasks>>(data, true, null);
            }
            catch (Exception ex)
            {
                return new OperationResult<IEnumerable<RabbitTasks>>(null, false, ex.Message);
            }
        }

        public async Task<OperationResult<RabbitTasks>> ReadAsync(int Id, params Expression<Func<RabbitTasks, object>>[] includes)
        {
            //Sql sql = new Sql()
            //{
            //    Text = $"Select top 1 * from RabbitTasks where id  = {Id}",
            //    commandType = System.Data.CommandType.Text
            //};

            try
            {
                RabbitTasks? data = await facad.repository.GetAsync<RabbitTasks>(p => p.Id == Id,includes);
                return new OperationResult<RabbitTasks>(data,true, null);
            }
            catch (Exception ex)
            {
                return new OperationResult<RabbitTasks>(false, ex.Message);
            }
        }

        public async Task<OperationResult> UpdateAsync(RabbitTasks taskModels)
        {
            //Sql sql = new Sql()
            //{
            //    Text = "Sp_RabbitTasks_Update",
            //    commandType = System.Data.CommandType.StoredProcedure,
            //    Parameters = new
            //    {
            //        taskModels.Id,
            //        taskModels.TaskName,
            //        taskModels.ProccessorName,
            //        taskModels.IntervalHour,
            //        taskModels.IntervalMinute,
            //        taskModels.FromDateTime,
            //        taskModels.ToDateTime,
            //        taskModels.RoutingKey
            //    },
            //    transaction = null
            //};
            try
            {
                facad.repository.Update(taskModels);
                await facad.SaveChangeAsync();
                return new OperationResult();
            }
            catch (Exception ex)
            {
                return new OperationResult(false, ex.Message);
            }
        }
    }
}
