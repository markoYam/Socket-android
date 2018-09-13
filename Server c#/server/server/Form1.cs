using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.DirectoryServices;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static server.Form1;

namespace server
{
    public partial class Form1 : Form, result
    {
        private String patch = null;
       
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSeleccionar_Click(object sender, EventArgs e)
        {
            //pickFile();
           comunicacion com = new comunicacion();
           com.StartServer(this);
        }
        private void pickFile()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();



            openFileDialog1.InitialDirectory = @"C:\";

            openFileDialog1.Title = "Buscar archivos";



            openFileDialog1.CheckFileExists = true;

            openFileDialog1.CheckPathExists = true;



            openFileDialog1.DefaultExt = "txt";

            openFileDialog1.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            openFileDialog1.FilterIndex = 2;

            openFileDialog1.RestoreDirectory = true;



            openFileDialog1.ReadOnlyChecked = true;

            openFileDialog1.ShowReadOnly = true;



            if (openFileDialog1.ShowDialog() == DialogResult.OK)

            {

                lblRuta.Text = openFileDialog1.FileName;

            }
        }
        private void button1_Click(object sender, System.EventArgs e)
        {
            comunicacion com = new comunicacion();
            com.StartServer(this);
        }

        public void onsucces(string mensaje)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate ()
                {
                    lblMensaje.Text = lblMensaje.Text + "\n exito: " + mensaje;
                }
                );
            }
           
        }

        public void onFailed(string mensaje)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate ()
                {
                    lblMensaje.Text = lblMensaje.Text + "\n error: " + mensaje;
                }
                );
            }

        }

        public class comunicacion
        {
            TcpListener tcpListener;
            result mres;
            public bool StartServer(result mres)
            {
                this.mres = mres;
                try
                {
                    tcpListener = new TcpListener(IPAddress.Any, 5678);
                    tcpListener.Start();
                    tcpListener.BeginAcceptTcpClient(new AsyncCallback(ProcessEvents), tcpListener);
                    Console.WriteLine("Listing at Port " + 5678);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.ToString());
                    return false;
                }
                return true;
            }

            void ProcessEvents(IAsyncResult asyn)
            {
                try
                {
                    TcpListener processListen = (TcpListener)asyn.AsyncState;
                    TcpClient tcpClient = processListen.EndAcceptTcpClient(asyn);
                    NetworkStream myStream = tcpClient.GetStream();
                    if (myStream.CanRead)
                    {
                        StreamReader readerStream = new StreamReader(myStream);
                        string myMessage = readerStream.ReadToEnd();
                        Console.WriteLine("--------");
                        mres.onsucces("Mensaje recivido: " + myMessage);
                        mres.onsucces("Ip " + ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString());
                        // Console.WriteLine("Mensaje recivido: " + myMessage);
                        //Console.WriteLine("Ip: " + ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString());
                        if (myMessage == "bloc")
                        {
                            //bloquear la pc
                            Process.Start(@"C:\WINDOWS\system32\rundll32.exe", "user32.dll,LockWorkStation");

                          
                            // writeData(myStream, "Hola client!",mres);
                        }
                        if(myMessage.Contains("pass:"))
                        {
                            String[] content = myMessage.Split(':');
                            //0 -> cadena de reconocimento
                            //1 -> nombre
                            //2 -> pass
                            //  writeData(myStream, "Hola client",mres);
                            DirectoryEntry AD = new DirectoryEntry("WinNT://" + Environment.MachineName + ",computer");
                            DirectoryEntry grp;
                            grp = AD.Children.Find(content[1], "user");
                            if (grp != null)
                            {
                                grp.Invoke("SetPassword", new object[] { content[2] });
                            }
                            grp.CommitChanges();
                            //MessageBox.Show("Account Change password Successfully");
                            Process.Start(@"C:\WINDOWS\system32\rundll32.exe", "user32.dll,LockWorkStation");
                        }
                        else
                        {
                         
                        }
                        readerStream.Close();
                    }
                    myStream.Close();
                    tcpClient.Close();
                    tcpListener.BeginAcceptTcpClient(new AsyncCallback(ProcessEvents), tcpListener);
                }
                catch (Exception e)
                {
                    Console.WriteLine("error 2 " + e.ToString());
                }
            }

            public static void writeData(NetworkStream networkStream, string dataToClient,result resInterfaz)
            {
                //Console.WriteLine("Procesando envio...");
                resInterfaz.onsucces("Procesando envio...");
                resInterfaz.onsucces("Mensaje enviado: "+dataToClient);
                //Console.WriteLine("Mensaje: " + dataToClient);
                Byte[] sendBytes = null;
                try
                {
                    sendBytes = Encoding.ASCII.GetBytes(dataToClient);
                    networkStream.Write(sendBytes, 0, sendBytes.Length);
                    networkStream.Flush();
                    Console.Write("Enviado correctamente");
                }
                catch (SocketException e)
                {
                    resInterfaz.onFailed("Ocurrio un error al enviar mensaje");
                    Console.Write("Ocurrio un error");
                    throw;
                }
            }
        }

        public interface result
        {
            void onsucces(String mensaje);
            void onFailed(String mensaje);
        }
        //private void sendFileSelected()
        //{
        //    try
        //    {

        //      //  System.out.println("S: Connecting...");

        //        ServerSocket serverSocket = new ServerSocket(SERVERPORT);
        //        System.out.println("S: Socket Established...");

        //        Socket client = serverSocket.accept();
        //        System.out.println("S: Receiving...");

        //        ObjectOutputStream put = new ObjectOutputStream(
        //                client.getOutputStream());

        //        String s = "adios.wav";
        //        String str = "C:/";
        //        String path = str + s;
        //        System.out.println("The requested file is path: " + path);
        //        System.out.println("The requested file is : " + s);
        //        File f = new File(path);
        //        if (f.isFile())
        //        {
        //            FileInputStream fis = new FileInputStream(f);

        //            byte[] buf = new byte[1024];
        //            int read;
        //            while ((read = fis.read(buf, 0, 1024)) != -1)
        //            {
        //                put.write(buf, 0, read);
        //                put.flush();
        //            }

        //            System.out.println("File transfered");
        //            client.close();
        //            serverSocket.close();
        //            fis.close();
        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        System.out.println("S: Error");
        //        e.printStackTrace();
        //    }
        //    finally
        //    {

        //    }
        //}
    }
}
