using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Drawing;
using System.Text;

public class HoloLens{
  private Socket cli;
  public HoloLens(String ip, int port){
    IPAddress sIP = IPAddress.Parse(ip);
    IPEndPoint nut = new IPEndPoint(sIP, port);
    cli = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    cli.Connect(nut);
  }


  //private void send_data(String message){
  //  byte[] messy = Encoding.ASCII.GetBytes(message);
  //  try{
  //    this.udp_sock.SendTo(messy, this.client_ip);
  //  }catch(Exception e){
  //    Console.WriteLine("[!] Exception thrown while sending data [!]\n");
  //  }
  ///}

  private void sendFile(String file){
    //try{
      this.cli.SendFile(file);
    //}catch(Exception e){
      //Console.WriteLine("[!] Exception thrown while sending data [!]\n");
    //}
  }

  public static void Main(String[] args){
    HoloLens holo = new HoloLens("127.0.0.1", 55555);
    holo.sendFile("wut.jpg");
  }


}
