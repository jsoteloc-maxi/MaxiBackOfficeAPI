using Maxi.BackOffice.Agent.Domain.Model;
using Maxi.BackOffice.Agent.Infrastructure.Contracts;
using Maxi.BackOffice.Agent.Infrastructure.Entities;
using Maxi.BackOffice.Agent.Infrastructure.UnitOfWork.SqlServer;
using Maxi.BackOffice.CrossCutting.Common.Common;
using Maxi.BackOffice.CrossCutting.Common.SqlServer;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Xml.Linq;
using Maxi.BackOffice.Agent.Infrastructure.UnitOfWork.Interfaces;

namespace Maxi.BackOffice.Agent.Infrastructure.Repositories
{
    public class CheckImagePendingRepository : ICheckImagePendingRepository
    {
        private readonly IAplicationContext _dbContext;
        private readonly IAppCurrentSessionContext _appCurrentSessionContext;
        private readonly CheckImagePendingEntity _entity;

        public CheckImagePendingRepository(IAplicationContext dbContext, IAppCurrentSessionContext appCurrentSessionContext)
        {
            _dbContext = dbContext;
            _appCurrentSessionContext = appCurrentSessionContext;
            _entity = new CheckImagePendingEntity();
        }

        public int ChecImageProcessed(int id)
        {
            
            var row = GetById(id);
            row.ProcessingDate = DateTime.Now;
            row.Path = string.Empty;
            var result= Update(row);
            _dbContext.SaveChanges();
            return result;
        }

        public int Delete(int id)
        {
            throw new NotImplementedException();
        }

        public CheckImagePendingEntity GetById(int id)
        {
            _entity.IdcheckImagePending = id;
            return _entity.GetById(_dbContext.GetConnection(), _dbContext.GetTransaction());
        }

        public CheckImagePendingEntity Insert(CheckImagePendingEntity row)
        {
            row.UserId = _appCurrentSessionContext.IdUser;
            row.Agent = _appCurrentSessionContext.IdAgent;
            row.CreateDate = DateTime.Now;
            row.ProcessingDate = DateTime.Now;
            row.IdcheckImagePending=row.Insert<CheckImagePendingEntity>(_dbContext.GetConnection(), _dbContext.GetTransaction());
            _dbContext.SaveChanges();
            return row;
        }

        public int Update(CheckImagePendingEntity row)
        {
            var result = row.Update(_dbContext.GetConnection(), _dbContext.GetTransaction());
            return result;
        }

        public void UploadImage(List<UploadFileDto> uploadChecks)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("st_InsertUploadFiles", _dbContext.GetConnection(), _dbContext.GetTransaction());
                cmd.CommandType = CommandType.StoredProcedure;
                string xml = CreateXmlFiles(uploadChecks).ToString();
                cmd.Parameters.AddWithValue("@UploadXML", xml);
                SqlParameter outputIdParam = new SqlParameter("@HasError", SqlDbType.Bit)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputIdParam);
                cmd.ExecuteNonQuery();
                _dbContext.SaveChanges();
            }
            catch(Exception ex)
            {

            }
           

        }
        private XElement CreateXmlFiles(List<UploadFileDto> uploadFilesDto)
        {
            var rules = new XElement("UploadFiles",
                                            from r in uploadFilesDto
                                            select new XElement("UploadFile",
                                                new XElement("IdReference", r.IdReference),
                                                new XElement("FileName", r.FileName),
                                                new XElement("FileGuid", r.FileGuid),
                                                new XElement("Extension", r.Extension),
                                                new XElement("IdStatus", r.IdStatus),
                                                new XElement("ExpirationDate", r.ExpirationDate != null ? r.ExpirationDate.Value.ToString("MM/dd/yyyy") : string.Empty),
                                                new XElement("IdDocumentType", r.IdDocumentType),
                                                new XElement("IdUser", r.IdUser),
                                                new XElement("IdDocumentImageType", r.idImgType),
                                                new XElement("LastChange_LastUserChange", _appCurrentSessionContext.IdUser),
                                                new XElement("LastChange_LastDateChange", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss")),
                                                new XElement("LastChange_LastIpChange", _appCurrentSessionContext.PcIdentifier),
                                                new XElement("LastChange_LastNoteChange", r.IdDocumentType != 69 ? "This file comes from Transfer (Aditional info module)" : r.idImgType == 1 ? "This is the Front Img of check" : "This is the Back Img if check"),
                                                new XElement("CreationDate", DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss")),
                                                new XElement("IdCountry", r.IdCountry ?? 0),
                                                new XElement("IdState", r.IdState ?? 0)
                                                )
                                     );
            return rules;
        }
    }
}
