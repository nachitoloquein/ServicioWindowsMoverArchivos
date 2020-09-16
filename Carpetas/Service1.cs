using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;

namespace Carpetas
{
    public partial class Service1 : ServiceBase
    {
        private System.Timers.Timer tmProcess = null;
        

        public bool procesando; 
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            var appSettings = ConfigurationManager.AppSettings;

            string tpo_exec = appSettings["tpo_exec_serv"] ?? "Not Found";
            tmProcess = new System.Timers.Timer();
            tmProcess.Interval = int.Parse(tpo_exec.ToString());
            tmProcess.Elapsed += new System.Timers.ElapsedEventHandler(tmProcess_Elapsed);
            tmProcess.Enabled = true;
            tmProcess.Start();
        }

        public void OnStart_test()
        {
            var appSettings = ConfigurationManager.AppSettings;

            string tpo_exec = appSettings["tpo_exec_serv"] ?? "Not Found";
            tmProcess = new System.Timers.Timer();
            tmProcess.Elapsed += new System.Timers.ElapsedEventHandler(tmProcess_Elapsed);
            tmProcess.Enabled = true;
            tmProcess.Start();
        }

        protected override void OnStop()
        {
            if (tmProcess != null)
            {
                tmProcess.Stop();
            }
        }
        void tmProcess_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ExecuteProcess();
        }
        private void ExecuteProcess()
        {
            tmProcess.Enabled = false;
            if (!procesando)
            {
                procesando = true;

                MoverArchivos();

                procesando = false;
            }
            OnStop();
        }
        void MoverArchivos()
        {
           
            var appSettings = ConfigurationManager.AppSettings;
            string origenDirectorio = appSettings["CarpetaOrigen"].ToString(); 
            string destinationPath = appSettings["CarpetaDestino"].ToString();

            DirectoryInfo di = new DirectoryInfo(origenDirectorio);

            if(!Directory.Exists(origenDirectorio))
            {
                Directory.CreateDirectory(origenDirectorio);
            }
            if(!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
            }

            foreach(var archivo in di.GetFiles("*", SearchOption.AllDirectories))
            {

                if (File.Exists(destinationPath + archivo.Name))
                {
                    File.SetAttributes(destinationPath + archivo.Name, FileAttributes.Normal);
                    File.Delete(destinationPath + archivo.Name);
                }

                File.Move(origenDirectorio + archivo.Name, destinationPath + archivo.Name);
                File.SetAttributes(destinationPath + archivo.Name, FileAttributes.Normal);
          
               
            }
            
        }
    }
}
