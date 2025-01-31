using Maxi.BackOffice.Agent.Infrastructure.Common;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Net;

namespace MaxiBackOfficeAPI.Controllers
{
    public class ConnectionController : ControllerBase
    {

        [HttpGet]
        public IActionResult Ping(string text)
        {
            //this.Request.User
            //HttpContext.Current.Request.

            string texto = string.Format("Ping from: {1} text:{0}", text, HttpContext.Connection.RemoteIpAddress);
            return Ok(texto);
        }


        [HttpGet, Route("TestRaise")]
        public int TestRaise()
        {
            return int.Parse("A");
        }


        [HttpGet, Route("TestIrd")]
        public int TestGenIrd()
        {

            var g = new TSI_IRDGenerator();

            //Se va a necesitar consulta para obtener la info de return de un cheque
            //En dephi estan estos campos
            //
            // CHK_CheckNumber
            // CHK_CheckNumberStr
            // CHK_Amount
            // CHK_ABACode
            // CHK_Account
            // CHK_MICRLine
            // CRI_DateReturned
            // RR_ASC_X9
            // RR_Description
            // CRI_BankRetReference
            // BF_DateSent
            // ABAC_ABACode


            //var sPath = @"C:\JGV\TSI\TIF\";
            var ldir = Maxi.BackOffice.CrossCutting.Common.Common.IOUtil.AppBaseDirForce("logs");
            var originalImg = Image.FromFile(ldir + "47330.tif");


            g.SetCheckImages(originalImg, originalImg);

            g.Text_MICR = "c5555c  a015463166a  55511100c";

            g.Text_ReturnReasonCode = "A";
            g.Text_ReturnReasonName = "NSF";
            g.Text_ReturnReasonAbbreviation = "NSF";

            //F2 Vertical
            g.Text_F2_RoutingNum = "123000013";
            g.Text_F2_InstBusinessDate = "04/19/2005";
            g.Text_F2_Secuence = "4600612210";

            //Vertical atras
            g.TxtB5.Add("[123000013] 04/19/2005");
            g.TxtB5.Add("4600612210");

            g.Text_F3_InstRoutingNum = "122000166";
            g.Text_F3_InstBusinessDate = "04/19/2005";
            g.Text_F3_Secuence = "6522433605";

            g.vgDebug = false;
            g.GenImagesIRD();



            if (g.IRDImageF != null)
            {
                g.SaveImage("F", ldir + "IRD T1 Front API.tif");

                //g.IRDImageF.Save(ldir + "IRD T1 Front API.tif", ImageFormat.Tiff);
                //TiffUtil.SaveToTiff(g.IRDImageF, sPath + "IRD T2 Front.tif");
                //TifUtil_03.SaveToTiff(g.IRDImageF, sPath + "IRD_Front T3.tif");
                //picBox3.Image = g.IRDImageF;
            }

            if (g.IRDImageR != null)
            {
                g.SaveImage("R", ldir + "IRD T1 Rear API.tif");

                //g.IRDImageR.Save(ldir + "IRD T1 Rear API.tif", ImageFormat.Tiff);
                //TiffUtil.SaveToTiff(g.IRDImageR, sPath + "IRD T2 Rear.tif");
                //TifUtil_03.SaveToTiff(g.IRDImageR, sPath + "IRD_Rear T3.tif");
                //picBox4.Image = g.IRDImageR;
            }


            return int.Parse("0");
        }

    }
}
