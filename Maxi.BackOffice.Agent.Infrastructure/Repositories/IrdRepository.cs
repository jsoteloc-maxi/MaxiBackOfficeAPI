using System.Dynamic;
using Maxi.BackOffice.CrossCutting.Common.Common;
using Maxi.BackOffice.Agent.Infrastructure.Common;
using Maxi.BackOffice.Agent.Infrastructure.Contracts;
using Maxi.BackOffice.Agent.Infrastructure.Entities;
using Maxi.BackOffice.Agent.Infrastructure.UnitOfWork.SqlServer;
using Maxi.BackOffice.Agent.Infrastructure.ExternalServices;
using Dapper;
using Maxi.BackOffice.Agent.Infrastructure.UnitOfWork.Interfaces;

namespace Maxi.BackOffice.Agent.Infrastructure.Repositories
{
    public class IrdRepository : IIrdRepository
    {
        private readonly IAplicationContext _dbContext;
        private readonly IAppCurrentSessionContext _appCurrentSessionContext;

        public IrdRepository(IAplicationContext dbContext, IAppCurrentSessionContext appCurrentSessionContext)
        {
            _dbContext = dbContext;
            _appCurrentSessionContext = appCurrentSessionContext;
        }

        // JISC TODO: REVISAR EL ERROR DEL METODO
        //****
        //public dynamic ObtenerImagenesIRD(int idCheckOri, bool ignoreDeny=false,bool IsIrdPrinted=true)
        //{
        //    GLogger.Debug("ObtenerImagenesIRD");

        //    dynamic result = new ExpandoObject();

        //    result.ErrorCode = 1;
        //    result.ErrorMsg = "";
        //    result.FrontBytes = null;
        //    result.RearBytes = null;

        //    try
        //    {
        //        var rejectInfo = _dbContext.GetConnection().QuerySingle<CheckRejectHistoryEntity>(
        //            "SELECT * FROM CheckRejectHistory WITH(NOLOCK) WHERE IdCheck = @IdCheck",
        //            new { IdCheck = idCheckOri },
        //            _dbContext.GetTransaction());

        //        if (rejectInfo == null)
        //        {
        //            var s = $"Reject Data Not Found for IdCheck={idCheckOri}";
        //            throw new Exception(s);
        //        }

        //        //Todo: ver si se pone otra bandera para separar cuando ya se imprimo y cuando se proceso en front 

        //        if (rejectInfo.IrdPrinted && !ignoreDeny)
        //        {
        //            var s = _dbContext.LangResource("IrdWasPrinted", "IRD WAS PRINTED");
        //            throw new Exception(s);
        //        }

        //        // JISC TODO
        //        var checkOri = _dbContext.GetEntityById<ChecksEntity>(idCheckOri);

        //        //Ruta para guardar IRD,  ver poner en attribute
        //        var irdPath = CrossCutting.Common.Common.IOUtil.AppBaseDirForce(@"logs\IRD");

        //        //nombres de archivos IRD
        //        var file_IRD_F = irdPath + checkOri.IdCheck.ToString() + "_IRD_F.tif";
        //        var file_IRD_R = irdPath + checkOri.IdCheck.ToString() + "_IRD_R.tif";


        //        //Si no existen las imagenes
        //        //if (!File.Exists(file_IRD_F) || !File.Exists(file_IRD_R) || string.IsNullOrWhiteSpace(rejectInfo.IrdMicr)) //comantado para siempre generar los archivos
        //        {
        //            rejectInfo.DateOfReject = DateTime.Now;
        //            var pic = CreateImagesIrd(checkOri, rejectInfo);

        //            File.WriteAllBytes(file_IRD_F, pic.FrontBytes);
        //            File.WriteAllBytes(file_IRD_R, pic.RearBytes);
        //            rejectInfo.IrdMicr = pic.Micr;
        //        }

        //        result.FrontBytes = File.ReadAllBytes(file_IRD_F);
        //        result.RearBytes = File.ReadAllBytes(file_IRD_R);
        //        result.IrdMicr = rejectInfo.IrdMicr;

        //        var sqlUpd = @"UPDATE dbo.CheckRejectHistory SET IrdPrinted=@IrdPrinted, IrdMicr=@IrdMicr WHERE IdCheckRejectHistory=@Id";
        //        _dbContext.GetConnection().Execute(sqlUpd, new { IrdMicr = rejectInfo.IrdMicr, Id = rejectInfo.IdCheckRejectHistory, IrdPrinted = IsIrdPrinted }, _dbContext.GetTransaction());

        //        DBSaveLog(rejectInfo.IdCheckRejectHistory);
        //        result.ErrorCode = 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        GLogger.Error(ex);
        //        result.ErrorCode = 3;
        //        result.ErrorMsg = ex.Message;
        //    }

        //    return result;
        //}

        // JISC TODO: REVISAR EL ERROR DEL METODO
        //private dynamic CreateImagesIrd(ChecksEntity checkInfo, CheckRejectHistoryEntity rejectInfo)
        //{
        //    GLogger.Debug("CreateImagesIrd");

        //    //Caracteres separadores segun el font MICR
        //    var cOnUs = 'c';
        //    var cTransit = 'a';
        //    var cAmount = 'b';

        //    string vlMicr = "";
        //    string vlAmount = "";
        //    string vlRouting = ""; //TransitField
        //    string vlAccount = ""; //OnUsField
        //    string vlCheckNum = "";
        //    string vlRetDate = "";
        //    string vlRetReasonCode = "";   //ASC_X9
        //    string vlRetReasonDesc = "";   //
        //    string vlRetReasonAbbr = "";   //
        //    string vlSentToBankDate = "";
        //    string vlBankABACode = "";
        //    string vlBankRef = "";
        //    string vlMaxiRef = "";

        //    var rrInfo = _dbContext.GetEntityById<CC_ReturnedReasonsEntity>(rejectInfo.IdReturnedReason);
        //    if (rrInfo == null)
        //    {
        //        var s = $"ReturnedReason_ID not found {rejectInfo.IdReturnedReason}";
        //        throw new Exception(s);
        //    }

        //    vlRetReasonCode = rrInfo.RTR_ASCX9 ?? "";
        //    vlRetReasonDesc = rrInfo.RTR_X9ShortName ?? "";
        //    vlRetReasonAbbr = rrInfo.RTR_X9Abbreviation ?? "";

        //    if (string.IsNullOrWhiteSpace(vlRetReasonCode)) throw new Exception("REASON X9 CODE EMPTY");
        //    if (string.IsNullOrWhiteSpace(vlRetReasonDesc)) throw new Exception("REASON X9 NAME EMPTY");
        //    if (string.IsNullOrWhiteSpace(vlRetReasonAbbr)) throw new Exception("REASON X9 ABBR EMPTY");

        //    vlCheckNum = checkInfo.CheckNumber ?? "";
        //    //if (int.TryParse(vlCheckNum, out var n)) vlCheckNum = n.ToString();

        //    vlAmount = string.Format("{0:00000000.00}", checkInfo.Amount); //10car
        //    vlAmount = vlAmount.Replace(".", "");
        //    vlRouting = checkInfo.RoutingNumber;
        //    vlAccount = checkInfo.Account;


        //    int vlMicrType = 1;

        //    vlMicr = checkInfo.MicrManual ?? "";
        //    if (string.IsNullOrWhiteSpace(vlMicr)) vlMicr = checkInfo.MicrOriginal ?? "";
        //    vlMicr = vlMicr.Trim().PadRight(1);

        //    //Todo: cual es el caracter en letra para < !!
        //    if (vlMicr[0] == '<' || vlMicr[0] == 'c' || vlMicr[0] == 'C')
        //        vlMicrType = 2;

        //    if (rejectInfo.DateOfReject == null)
        //        throw new Exception("REJECTED DATE IS NULL");

        //    /*07-Sep-2021*/
        //    /*UCF*/
        //    /*TSI_MAXI_013*/
        //    /*Se usa la fecha actual al generar la imagen del IRD*/
        //    vlRetDate = rejectInfo.DateOfReject.Value.ToString("MM/dd/yyyy");
        //    //vlRetDate = _dbContext.GetConnection().ExecuteScalar<DateTime>("SELECT GETDATE()").ToString("MM/dd/yyyy");

        //    vlBankRef = checkInfo.IdCheck.ToString(); //En base original, CRI_BankReference es el id del cheque
        //                                              //vlBankABACode  aba del banco al que se envió

        //    {
        //        var r = _dbContext.GetEntityById<CheckProcessorBankEntity>(checkInfo.IdCheckProcessorBank);
        //        if (r == null)
        //            throw new Exception($"Create Image IRD, CheckProcessorBank {checkInfo.IdCheckProcessorBank} not found for IdCheck={checkInfo.IdCheck}");
        //        vlBankABACode = r.ABACode ?? "";
        //    }


        //    //Fecha de status Paid, se usa para fecha cuando se envia al banco
        //    var datePaid = _dbContext.GetConnection().ExecuteScalar<DateTime>(
        //        "SELECT CD.DateOfMovement " +
        //        "FROM CheckDetails CD WITH(NOLOCK) " +
        //        "JOIN [Status] ST WITH(NOLOCK) ON ST.IdStatus = CD.IdStatus " +
        //        "WHERE ST.StatusName='Paid' AND CD.IdCheck = @IdCheck ",
        //        new { IdCheck = checkInfo.IdCheck },
        //        _dbContext.GetTransaction());

        //    if (datePaid == null)
        //        throw new Exception("CHECK PAID DATE IS NULL OR NOT FOUND");

        //    vlSentToBankDate = datePaid.ToString("MM/dd/yyyy");  //BF_DateSent mm/dd/yyyy  fecha cheque original enviado al banco


        //    vlRouting = vlRouting.PadLeft(9).Replace(' ', '0');
        //    vlAccount = vlAccount.PadLeft(12);
        //    vlBankRef = vlBankRef.PadLeft(16).Replace(' ', '0');
        //    vlMaxiRef = checkInfo.IdCheck.ToString().PadLeft(15).Replace(' ', '0');

        //    if (vlMicrType == 1)
        //    {
        //        //PersonalCheck
        //        vlMicr = "4" + cTransit + vlRouting + cTransit + " ";
        //        vlMicr += vlAccount + cOnUs + vlCheckNum.PadLeft(4, '0').PadRight(8);
        //        vlMicr += cAmount + vlAmount + cAmount;
        //    }
        //    else
        //    {
        //        //BusinessCheck
        //        vlMicr = cOnUs + vlCheckNum.PadLeft(6).Replace(" ", "0") + cOnUs + " ";
        //        vlMicr += "4" + cTransit + vlRouting + cTransit;
        //        vlMicr += vlAccount + cOnUs + string.Empty.PadRight(8);
        //        vlMicr += cAmount + vlAmount + cAmount;
        //    }

        //    string file_Check_F = "";
        //    string file_Check_R = "";
        //    try
        //    {
        //        GetOriginalCheckImages(checkInfo.IdIssuer, checkInfo.IdCheck, out file_Check_F, out file_Check_R);
        //    }
        //    catch (Exception ex)
        //    {
        //        GLogger.Error(ex);
        //        var s = _dbContext.LangResource("CheckImageNotFound", "Original Check Images not Found");
        //        throw new Exception(s);
        //    }


        //    var g = new TSI_IRDGenerator();
        //    g.LoadCheckImage("F", file_Check_F);
        //    g.LoadCheckImage("R", file_Check_R);

        //    g.Text_MICR = vlMicr;

        //    g.Text_ReturnReasonCode = vlRetReasonCode;
        //    g.Text_ReturnReasonName = vlRetReasonDesc;
        //    g.Text_ReturnReasonAbbreviation = vlRetReasonAbbr;
        //    //g.TxtB3.Add("Return Reason " + vlRetReasonCode);
        //    //g.TxtB3.Add(vlRetReasonDesc);
        //    //Return Reason sobre el cheque
        //    //g.TxtB4.Add("Return Reason " + vlRetReasonCode);
        //    //g.TxtB4.Add(vlRetReasonDesc);


        //    //Texto vertical
        //    g.Text_F2_RoutingNum = vlBankABACode;
        //    g.Text_F2_InstBusinessDate = vlSentToBankDate;
        //    g.Text_F2_Secuence = vlMaxiRef;

        //    //Vertical atras
        //    g.TxtB5.Add($"[{vlBankABACode}] {vlSentToBankDate}");
        //    g.TxtB5.Add(vlMaxiRef);

        //    //
        //    g.Text_F3_InstRoutingNum = vlBankABACode;
        //    g.Text_F3_InstBusinessDate = vlRetDate;
        //    g.Text_F3_Secuence = vlBankRef;
        //    //g.TxtB1.Add("*" + vlBankABACode + "*");
        //    //g.TxtB1.Add(vlRetDate);
        //    //g.TxtB1.Add(vlBankRef);

        //    g.vgDebug = false;
        //    g.GenImagesIRD();

        //    if (g.IRDImageF == null) throw new Exception("IRD front image not generated.");
        //    if (g.IRDImageR == null) throw new Exception("IRD rear image not generated.");

        //    var mF = g.SaveImageToStream("F");
        //    var mR = g.SaveImageToStream("R");

        //    return new
        //    {
        //        FrontBytes = mF.ToArray(),
        //        RearBytes = mR.ToArray(),
        //        Micr = vlMicr,
        //    };
        //}

        // JISC TODO: REVISAR EL ERROR DEL METODO
        //private dynamic GenerarIrd_Deprecated(int idCheck)
        //{
        //    GLogger.Debug("GenerarIrd");

        //    dynamic result = new ExpandoObject();

        //    result.ErrorCode = 1;
        //    result.ErrorMsg = "";
        //    result.FrontBytes = null;
        //    result.RearBytes = null;

        //    try
        //    {
        //        //Caracteres separadores segun el font MICR
        //        var cOnUs = 'c';
        //        var cTransit = 'a';
        //        var cAmount = 'b';

        //        string vlMicr = "";
        //        string vlAmount = "";
        //        string vlRouting = ""; //TransitField
        //        string vlAccount = ""; //OnUsField
        //        string vlCheckNum = "";
        //        string vlRetDate = "";
        //        string vlRetReasonCode = "";   //ASC_X9
        //        string vlRetReasonDesc = "";   //
        //        string vlRetReasonAbbr = "";   //
        //        string vlSentToBankDate = "";
        //        string vlBankABACode = "";
        //        string vlBankRef = "";
        //        string vlMaxiRef = "";

        //        var checkInfo = _dbContext.GetEntityById<ChecksEntity>(idCheck);


        //        var rejectInfo = _dbContext.GetConnection().QuerySingle<CheckRejectHistoryEntity>(
        //            "SELECT * FROM CheckRejectHistory WITH(NOLOCK) WHERE IdCheck = @IdCheck",
        //            new { IdCheck = idCheck },
        //            _dbContext.GetTransaction());

        //        // Si ya fue generado anteriormente => no lo permite regresar
        //        if (rejectInfo.IrdPrinted == true)
        //        {
        //            var s = _dbContext.LangResource("IrdWasPrinted", "IRD WAS PRINTED");
        //            throw new Exception(s);
        //        }

        //        var rrInfo = _dbContext.GetEntityById<CC_ReturnedReasonsEntity>(rejectInfo.IdReturnedReason);

        //        //var rrInfo = _dbContext.GetConnection().QueryFirstOrDefault<CC_ReturnedReasonsEntity>(
        //        //    "SELECT * FROM dbo.CC_ReturnedReasons WITH(nolock) WHERE ReturnedReason_ID = @Id",
        //        //    new { Id = rejectInfo.IdReturnedReason },
        //        //    _dbContext.GetTransaction());

        //        if(rrInfo == null)
        //        {
        //            var s = $"ReturnedReason_ID not found {rejectInfo.IdReturnedReason}";
        //            throw new Exception(s);
        //        }

        //        vlRetReasonCode = rrInfo.RTR_ASCX9 ?? "";
        //        vlRetReasonDesc = rrInfo.RTR_X9ShortName ?? "";
        //        vlRetReasonAbbr = rrInfo.RTR_X9Abbreviation ?? "";


        //        //Ruta para guardar IRD,  ver poner en attribute
        //        var irdPath = CrossCutting.Common.Common.IOUtil.AppBaseDirForce(@"logs/IRD");
                
        //        //nombres de archivos IRD
        //        var file_IRD_F = irdPath + checkInfo.IdCheck.ToString() + "_IRD_F.tif";
        //        var file_IRD_R = irdPath + checkInfo.IdCheck.ToString() + "_IRD_R.tif";

        //        //Si ya existen las imagenes las regresa
        //        if (File.Exists(file_IRD_F) && File.Exists(file_IRD_R))
        //        {
        //            result.FrontBytes = File.ReadAllBytes(file_IRD_F);
        //            result.RearBytes = File.ReadAllBytes(file_IRD_R);
        //            result.ErrorCode = 0;
                    
        //            DBSaveLog(rejectInfo.IdCheckRejectHistory);
        //            return result;
        //        }

        //        if (string.IsNullOrWhiteSpace(vlRetReasonCode)) throw new Exception("REASON X9 CODE EMPTY");
        //        if (string.IsNullOrWhiteSpace(vlRetReasonDesc)) throw new Exception("REASON X9 NAME EMPTY");
        //        if (string.IsNullOrWhiteSpace(vlRetReasonAbbr)) throw new Exception("REASON X9 ABBR EMPTY");


        //        vlCheckNum = checkInfo.CheckNumber ?? "";
        //        if (int.TryParse(vlCheckNum, out var n)) vlCheckNum = n.ToString();

        //        vlAmount = string.Format("{0:0.00}", checkInfo.Amount);
        //        vlAmount = vlAmount.Replace(".", "");
        //        vlRouting = checkInfo.RoutingNumber;
        //        vlAccount = checkInfo.Account;


        //        int vlMicrType = 1;

        //        vlMicr = checkInfo.MicrManual ?? "";
        //        if (string.IsNullOrWhiteSpace(vlMicr)) vlMicr = checkInfo.MicrOriginal ?? "";
        //        vlMicr = vlMicr.Trim().PadRight(1);

        //        //Todo: cual es el caracter en letra para < !!
        //        if (vlMicr[0] == '<' || vlMicr[0] == 'c' || vlMicr[0] == 'C')
        //            vlMicrType = 2;

        //        if (rejectInfo.DateOfReject == null)
        //            throw new Exception("REJECTED DATE IS NULL");

        //        vlRetDate = rejectInfo.DateOfReject.Value.ToString("MM/dd/yyyy");

                
        //        vlBankRef = checkInfo.IdCheck.ToString(); //En base original, CRI_BankReference es el id del cheque
        //        //vlBankABACode  aba del banco al que se envió

        //        {
        //            var r = _dbContext.GetEntityById<CheckProcessorBankEntity>(checkInfo.IdCheckProcessorBank);
        //            if (r == null)
        //                throw new Exception($"CheckProcessorBank {checkInfo.IdCheckProcessorBank} not found");
        //            vlBankABACode = r.ABACode ?? "";
        //        }
                    
                
        //        //Fecha de status Paid, se usa para fecha cuando se envia al banco
        //        var datePaid = _dbContext.GetConnection().ExecuteScalar<DateTime>(
        //            "SELECT CD.DateOfMovement " +
        //            "FROM CheckDetails CD WITH(NOLOCK) " +
        //            "JOIN [Status] ST WITH(NOLOCK) ON ST.IdStatus = CD.IdStatus " +
        //            "WHERE ST.StatusName='Paid' AND CD.IdCheck = @IdCheck ",
        //            new { IdCheck = idCheck },
        //            _dbContext.GetTransaction());

        //        if(datePaid==null)
        //            throw new Exception("PAID DATE IS NULL");

        //        vlSentToBankDate = datePaid.ToString("MM/dd/yyyy");  //BF_DateSent mm/dd/yyyy  fecha cheque original enviado al banco


        //        vlRouting = vlRouting.PadLeft(9).Replace(' ', '0');
        //        vlAccount = vlAccount.PadLeft(12);
        //        vlBankRef = vlBankRef.PadLeft(16).Replace(' ', '0');
        //        vlMaxiRef = checkInfo.IdCheck.ToString().PadLeft(15).Replace(' ', '0');

        //        if (vlMicrType == 1)
        //        {
        //            //PersonalCheck
        //            vlMicr = "4" + cTransit + vlRouting + cTransit + " ";
        //            vlMicr += vlAccount + cOnUs + vlCheckNum.PadLeft(4, '0').PadRight(8);
        //            vlMicr += cAmount + vlAmount + cAmount;
        //        }
        //        else
        //        {
        //            //BusinessCheck
        //            vlMicr = cOnUs + vlCheckNum.PadLeft(6).Replace(" ", "0") + cOnUs + " ";
        //            vlMicr += "4" + cTransit + vlRouting + cTransit;
        //            vlMicr += vlAccount + cOnUs + string.Empty.PadRight(8);
        //            vlMicr += cAmount + vlAmount + cAmount;
        //        }

        //        string file_Check_F = "";
        //        string file_Check_R = "";
        //        try
        //        {
        //            //file_Check_F = @"C:\JGV\TSI\TIF\47330.tif";
        //            //file_Check_R = file_Check_F;

        //            GetOriginalCheckImages(checkInfo.IdIssuer, checkInfo.IdCheck, out file_Check_F, out file_Check_R);
        //        }
        //        catch(Exception ex)
        //        {
        //            GLogger.Error(ex);
        //            var s = _dbContext.LangResource("CheckImageNotFound", "Imagen de cheque no encontrada");
        //            throw new Exception(s);
        //        }

                

        //        var g = new TSI_IRDGenerator();
        //        g.LoadCheckImage("F", file_Check_F);
        //        g.LoadCheckImage("R", file_Check_R);

        //        g.Text_MICR = vlMicr;

        //        g.Text_ReturnReasonCode = vlRetReasonCode;
        //        g.Text_ReturnReasonName = vlRetReasonDesc;
        //        g.Text_ReturnReasonAbbreviation = vlRetReasonAbbr;
        //        //g.TxtB3.Add("Return Reason " + vlRetReasonCode);
        //        //g.TxtB3.Add(vlRetReasonDesc);
        //        //Return Reason sobre el cheque
        //        //g.TxtB4.Add("Return Reason " + vlRetReasonCode);
        //        //g.TxtB4.Add(vlRetReasonDesc);


        //        //Texto vertical
        //        g.Text_F2_RoutingNum = vlBankABACode;
        //        g.Text_F2_InstBusinessDate = vlSentToBankDate;
        //        g.Text_F2_Secuence = vlMaxiRef;

        //        //Vertical atras
        //        g.TxtB5.Add($"[{vlBankABACode}] {vlSentToBankDate}");
        //        g.TxtB5.Add(vlMaxiRef);

        //        //
        //        g.Text_F3_InstRoutingNum = vlBankABACode;
        //        g.Text_F3_InstBusinessDate = vlRetDate;
        //        g.Text_F3_Secuence = vlBankRef;
        //        //g.TxtB1.Add("*" + vlBankABACode + "*");
        //        //g.TxtB1.Add(vlRetDate);
        //        //g.TxtB1.Add(vlBankRef);

        //        g.vgDebug = false;
        //        g.GenImagesIRD();

        //        if (g.IRDImageF == null) throw new Exception("IRD front image not generated.");
        //        if (g.IRDImageR == null) throw new Exception("IRD rear image not generated.");

        //        //var cname = checkInfo.IdCheck.ToString() + "_IRD_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");

        //        g.SaveImage("F", file_IRD_F);
        //        g.SaveImage("R", file_IRD_R);

        //        result.FrontBytes = File.ReadAllBytes(file_IRD_F);
        //        result.RearBytes = File.ReadAllBytes(file_IRD_R);

        //        //Si estan grabadas las imagenes => es importante
        //        //Marcar el registro rejected de que ya genero ird
        //        //grabar log de que se genero

        //        //var x = new CheckRejectHistoryEntity();
        //        //x.IdCheckRejectHistory
        //        //x.GetById(_dbContext.conn, _dbContext.tran);

        //        var sqlUpd = @"UPDATE dbo.CheckRejectHistory SET IrdPrinted=1 WHERE IdCheckRejectHistory=@Id";
        //        _dbContext.GetConnection().Execute(sqlUpd, new { Id = rejectInfo.IdCheckRejectHistory }, _dbContext.GetTransaction());

        //        DBSaveLog( rejectInfo.IdCheckRejectHistory );

        //        result.ErrorCode = 0;
        //    }
        //    catch (Exception e)
        //    {
        //        result.ErrorCode = 3;
        //        result.ErrorMsg = e.Message;
        //        GLogger.Debug(result.ErrorMsg);
        //    }

        //    return result;
        //}


        void DBSaveLog(int idReject)
        {
            var sqlIns = @"INSERT INTO MAXILOG.dbo.IRDProcessLog Values( @Event, GETDATE(), @IdUser, @IdAgent, @IdReject )";
            _dbContext.GetConnection().Execute(sqlIns, new
            {
                Event = "",
                IdUser = _appCurrentSessionContext.IdUser,
                IdAgent = _appCurrentSessionContext.IdAgent,
                IdReject = idReject,
            }, _dbContext.GetTransaction());
        }


        void GetOriginalCheckImages(int idIssuer, int idCheck, out string fileFront, out string fileRear)
        {
            GLogger.Debug("GetOriginalCheckImages For IRD");

            //obtiene los nombre de archivo de las imagenes frontal y trasera a partir de la tabla
            //UploadFiles

            var _path = AppDomain.CurrentDomain.BaseDirectory + @"\logs\";
            if (_path.ToUpper().StartsWith(@"C:\JGV\"))
            {
                fileFront = _path + "47330.tif";
                fileRear = _path + "47330.tif";
                return;
            }

            GLogger.Debug($"GetOriginalCheckImages  idIssuer:{idIssuer}  idCheck:{idCheck}");
            fileFront = "";
            fileRear = "";

            string sqlImg =
                " SELECT FH.FileName, FH.FileGuid, FH.Extension " +
                " FROM UploadFiles FH WITH(NOLOCK) " +
                " JOIN UploadFilesDetail FD WITH(NOLOCK) ON FH.IdUploadFile = FD.IdUploadFile " +
                " WHERE FH.IdReference = @IdCheck " +
                " AND FH.IdDocumentType = 69 " +
                " AND FD.IdDocumentImageType = @ImageType ";

            try
            {
                var picF = _dbContext.GetConnection().QueryFirstOrDefault(sqlImg, new { IdCheck = idCheck, ImageType = 1 }, _dbContext.GetTransaction());
                var picB = _dbContext.GetConnection().QueryFirstOrDefault(sqlImg, new { IdCheck = idCheck, ImageType = 2 }, _dbContext.GetTransaction());
                if (picF == null || picB == null)
                    throw new Exception($"Check Images Not Found in Table UploadFiles, IdCheck={idCheck}");

                var path = _dbContext.GlobalAttr("ChecksPath");
                path = Path.Combine(path, idIssuer.ToString(), "Checks", idCheck.ToString());
                path += @"\";

                GLogger.Debug("Ruta: " + path);
                if(!Directory.Exists(path))
                    throw new Exception("ChecksPath does not exists");

                fileFront = path + picF.FileName + picF.Extension;
                fileRear = path + picB.FileName + picB.Extension;

                #region ...
                /*
                var dirInfo = new DirectoryInfo(path);
                var files = dirInfo.GetFiles("*.tif");
                foreach (var f in files)
                {
                    if (iFront > 0)
                    {
                        iRear = f.Length;
                        fileRear = f.FullName;

                        if (iRear > iFront)
                        {
                            fileRear = fileFront;
                            fileFront = f.FullName;
                        }
                        break;
                    }

                    iFront = f.Length;
                    fileFront = f.FullName;
                }
                */
                #endregion

                GLogger.Debug("  Front : " + fileFront);
                GLogger.Debug("  Back : " + fileRear);

                if (!File.Exists(fileFront)) throw new Exception("File not found, check FRONT");
                if (!File.Exists(fileRear)) throw new Exception("File not found, check BACK");
            }
            catch(Exception ex)
            {
                throw new Exception("Error GetCheckImages: " + ex.Message);
            }
        }

        // JISC TODO: REVISAR EL ERROR DEL METODO
        //private Dictionary<string, string> GetReportParams(int idCheckOrig, string logoFile)
        //{
        //    //Crea diccionario con los parametros que requiere el RDL para imprimir IRD

        //    GLogger.Debug(">>>GetReportParams Entra");

        //    var r = new Dictionary<string, string>();

        //    GLogger.Debug($"  GetEntityById<AgentEntity>( {_appCurrentSessionContext.IdAgent} )");
        //    var agent = _dbContext.GetEntityById<AgentEntity>(_appCurrentSessionContext.IdAgent);
        //    if (agent == null) throw new Exception($"IdAgent ({_appCurrentSessionContext.IdAgent}) not found");

        //    GLogger.Debug($"  GetEntityByFilter<CheckRejectHistoryEntity>  idCheckOrig={idCheckOrig} ");
        //    var rejectInf = _dbContext.GetEntityByFilter<CheckRejectHistoryEntity>(
        //        "IdCheck=@idCheckOrig",
        //        new { idCheckOrig }).FirstOrDefault();
        //    if (rejectInf == null) throw new Exception($"RejectHistory ({idCheckOrig}) not found");

        //    var reasonInf = _dbContext.GetEntityById<CC_ReturnedReasonsEntity>(rejectInf.IdReturnedReason);
        //    if (reasonInf == null) throw new Exception($"IdReturnedReason ({rejectInf.IdReturnedReason}) not found");

        //    var checkOrig = _dbContext.GetEntityById<ChecksEntity>(idCheckOrig);
        //    if (checkOrig == null) throw new Exception($"IdCheckOrig ({idCheckOrig}) not found");

        //    r.Add("AgentName", agent.AgentName);
        //    r.Add("AgentAddress", agent.AgentAddress);
        //    r.Add("AgentLocation", "");
        //    r.Add("AgentPhone", agent.AgentPhone);
        //    r.Add("CorpPhone", "");
        //    r.Add("UserLogin", _appCurrentSessionContext.UserName);
        //    r.Add("RejectDate", rejectInf.DateOfReject?.ToString("MM/dd/yyyy"));
        //    r.Add("RejectReason", $"({reasonInf.RTR_ASCX9}) {reasonInf.RTR_X9ShortName}");
        //    r.Add("CheckNum", checkOrig.CheckNumber);

        //    var pic = ObtenerImagenesIRD(idCheckOrig);
        //    if (pic.ErrorCode != 0) throw new Exception(pic.ErrorMsg);

        //    var s = Convert.ToBase64String(pic.FrontBytes);
        //    r.Add("CheckImgF", s);

        //    s = Convert.ToBase64String(pic.RearBytes);
        //    r.Add("CheckImgR", s);

        //    r["LogoImg"] = "";
        //    if (File.Exists(logoFile))
        //        r["LogoImg"] = Convert.ToBase64String(File.ReadAllBytes(logoFile));


        //    GLogger.Debug("<<<GetReportParams Sale");
        //    return r;
        //}

        // JISC TODO: REVISAR EL ERROR DEL METODO
        //***
        //public dynamic ObtenerImpresionIRDParams(int idCheckOrig, string logoFile)
        //{
        //    GLogger.Debug("ObtenerImpresionIRDParams");

        //    dynamic result = new ExpandoObject();
        //    result.ErrorCode = 1;
        //    result.ErrorMsg = "";
        //    result.FrontBytes = null;
        //    result.RearBytes = null;
        //    result.RepParams = null;

        //    Dictionary<string, string> dParam;
        //    try
        //    {
        //        dParam = GetReportParams(idCheckOrig, logoFile);
        //        result.RepParams = dParam;
        //        result.ErrorCode = 0;

        //        //var paramsCopy = new Dictionary<string, string>(OldDictionary);

        //    }
        //    catch (Exception ex)
        //    {
        //        result.ErrorCode = 3;
        //        result.ErrorMsg = ex.Message;
        //    }

        //    return result;
        //}

        // JISC TODO: REVISAR EL ERROR DEL METODO
        //***
        //public dynamic ObtenerImpresionIRD(int idCheckOrig, string logoImg)
        //{
        //    GLogger.Debug(">>>ObtenerImpresionIRD Entra");

        //    dynamic result = new ExpandoObject();
        //    result.ErrorCode = 1;
        //    result.ErrorMsg = "";
        //    result.FrontBytes = null;
        //    result.RearBytes = null;

        //    Dictionary<string, string> dParam;
        //    try
        //    {
        //        GLogger.Debug($"  llama GetReportParams( {idCheckOrig}, {logoImg} )");
        //        dParam = GetReportParams(idCheckOrig, logoImg);
        //    }
        //    catch (Exception ex)
        //    {
        //        GLogger.Error(ex);
        //        result.ErrorCode = 3;
        //        result.ErrorMsg = ex.Message;
        //        GLogger.Debug($"<<<ObtenerImpresionIRD Sale");
        //        return result;
        //    }


        //    //var pic = ObtenerImagenesIRD(idCheckOrig);
        //    //if (pic.ErrorCode != 0)
        //    //{
        //    //    result.ErrorCode = pic.ErrorCode;
        //    //    result.ErrorMsg = pic.ErrorMsg;
        //    //    return result;
        //    //}

        //    //var agent = _dbContext.GetEntityById<AgentEntity>(session.IdAgent);
        //    //if (agent == null) throw new Exception($"IdAgent ({session.IdAgent}) not found");

        //    //var rejectInf = _dbContext.GetEntityByFilter<CheckRejectHistoryEntity>(
        //    //    "IdCheck=@IdCheck",
        //    //    new { IdCheck = idCheckOrig }).FirstOrDefault();

        //    //var reasonInf = _dbContext.GetEntityById<CC_ReturnedReasonsEntity>(rejectInf.IdReturnedReason);
        //    //if (reasonInf == null) throw new Exception($"IdReturnedReason ({rejectInf.IdReturnedReason}) not found");

        //    GLogger.Debug($"  crea-llena objeto param ");

        //    dynamic param = new ExpandoObject();

        //    GLogger.Debug($"  x1");
        //    param.AgentName = dParam["AgentName"];

        //    GLogger.Debug($"  x2");
        //    param.AgentAddress = dParam["AgentAddress"];

        //    GLogger.Debug($"  x3");
        //    param.AgentLocation = "";

        //    GLogger.Debug($"  x4");
        //    param.AgentPhone = dParam["AgentPhone"];

        //    GLogger.Debug($"  x5");
        //    param.CorpPhone = "";

        //    GLogger.Debug($"  x6");
        //    param.UserLogin = dParam["UserLogin"];

        //    //dParam.TryGetValue("UserName")

        //    GLogger.Debug($"  x7");
        //    param.RejectDate = dParam["RejectDate"];

        //    GLogger.Debug($"  x8");
        //    param.RejectReason = dParam["RejectReason"];

        //    GLogger.Debug($"  x9");
        //    param.CheckNum = dParam["CheckNum"];

        //    GLogger.Debug($"  x10");
        //    param.LogoImg = dParam["LogoImg"];

        //    GLogger.Debug("Llama Maxi CommonServices Report, IRD Front ");
        //    param.CheckSide = "F";
        //    param.CheckImg = dParam["CheckImgF"];
        //    var r1 = MaxiCommonServices.Report("CC_IRDReportCheckF", (object)param);

        //    GLogger.Debug("Llama Maxi CommonServices Report, IRD Rear ");
        //    param.CheckSide = "R";
        //    param.CheckImg = dParam["CheckImgR"];
        //    var r2 = MaxiCommonServices.Report("CC_IRDReportCheckF", (object)param);

        //    result.FrontBytes = r1.Bytes;
        //    result.RearBytes = r2.Bytes;

        //    //var path = AppDomain.CurrentDomain.BaseDirectory + @"\logs\";
        //    //var dummy = path + "dummyIRD.pdf";
        //    //result.DocPrint = File.ReadAllBytes(dummy);

        //    GLogger.Debug($"<<<ObtenerImpresionIRD Sale");
        //    result.ErrorCode = 0;
        //    return result;
        //}
    }

}
