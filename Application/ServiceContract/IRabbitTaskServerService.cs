using Application.ServiceImplement;
using Infrastructure.DataBaseRepository;
using Model.ApplicationModels;
using Model.PublicModels;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Security.AccessControl;
using System.Text;


namespace Application.ServiceContract
{
    public interface IRabbitTaskServerService
    {
        Task<OperationResult<IEnumerable<RabbitTaskServers>>> GetAllAsync(Func<RabbitTaskServers, bool>? Condition = null, params Expression<Func<RabbitTaskServers, object>>[] expressions);
        Task<OperationResult<Guid>> Delete(string Id);
        Task<OperationResult<RabbitTaskServers>> Update(RabbitTaskServers rabbitTaskServers, bool Concurrency = false);
    }

    public class RabbitTaskServerService : IRabbitTaskServerService
    {
        public RabbitTaskServerService(IFacadDesign facadDesign)
        {
            FacadDesign = facadDesign;
        }

        public IFacadDesign FacadDesign { get; }

        public async Task<OperationResult<Guid>> Delete(string Id)
        {
            try
            {
                FacadDesign.repository.Delete(Id);
                await FacadDesign.SaveChangeAsync();
                return new OperationResult<Guid>(Guid.Parse(Id), true, null);
            }
            catch (Exception ex)
            {
                return new OperationResult<Guid>(false, ex.Message);
            }
        }

        public async Task<OperationResult<IEnumerable<RabbitTaskServers>>> GetAllAsync(Func<RabbitTaskServers, bool>? Condition = null, params Expression<Func<RabbitTaskServers, object>>[] expressions)
        {
            try
            {
                var res = await FacadDesign.repository.GetAllAsync<RabbitTaskServers>(Condition, expressions);
                return new OperationResult<IEnumerable<RabbitTaskServers>>(res, true, null);
            }
            catch (Exception ex)
            {

                return new OperationResult<IEnumerable<RabbitTaskServers>>(false, ex.Message);

            }
        }


        public async Task<OperationResult<RabbitTaskServers>> Update(RabbitTaskServers rabbitTaskServers, bool Concurrency = false)
        {
            try
            {
                FacadDesign.repository.Update<RabbitTaskServers>(rabbitTaskServers, Concurrency);
                if (!Concurrency)
                    await FacadDesign.SaveChangeAsync();

                return new OperationResult<RabbitTaskServers>(rabbitTaskServers, true, null);
            }
            catch (Exception ex)
            {

                return new OperationResult<RabbitTaskServers>(false, ex.Message);

            }
        }
    }
}
