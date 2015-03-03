//A program to create custom POX flows
// https://github.com/abh15
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System;
using System.IO;

public class foo:Form
{
    public TextBox t,t2;
    public ComboBox cb1,cb2,cb3,cb4; // for access outside foo,e.g eventhandlers
    String s;
    TextBox[] me2 = new TextBox[13];    //array of type textbox 
    TextBox[] me3 = new TextBox[11];    
    List<string> rec_sw=new List<string>();
    List<string> rec_fl=new List<string>();
    int rec_count=0;
    
    void fieldmaker(int y,String[] sarray,TextBox[] varTb)  
    {                        //to create var match & action fields 
        int x=150;  //offset from top

        Label[] me = new Label[sarray.Length];      //create array of labels of len sarray.Length
          
        
    	for (int k = 0; k < sarray.Length;k++)   
    	{
    		var lbl=new Label();  //create label var lbl
            var txt=new TextBox(); //create TextBox var txt
    		me[k]=lbl;   //add label element to empty label array
            varTb[k]=txt;  //add elements of empty txtbox array
            
            lbl.Parent=this;            //Label attribs
    		lbl.AutoSize=true;
    		lbl.Text=sarray[k]; //set label text acc. to one in array passed to func
    		lbl.Location=new Point(60+y,x);
    		
            txt.Parent=this;           //Testbox attribs
            txt.AutoSize=true;
            
            txt.Location=new Point(170+y,x);
            x=x+40;          //increment offset per loop
 
            Controls.Add(txt);

    	}	
    }









	public foo()
{

    Text="POX Flow Generator";  //window title
    Size=new Size(720,700); //window size
   
String[] match_fields={"inport","dltype","nwtos","nwproto","nwsrc","nwdst","dlvlan","dlvlanpcp","dlsrc",
        "dldst","tpsrc","tpdst","priority"}; //match  array

String[] action_fields={"outport","vlanpriority","enqueue","srcport","dstport","srcmac","dstmac",
        "srcip","dstip","tos","vlanid"};   //  action array  
        

    fieldmaker(0,match_fields,me2);       //call fieldmaker to create match fields
    fieldmaker(300,action_fields,me3);  //call fieldmaker to create action fields


    Label L=new Label();       
    L.Parent=this;
    L.Text="Enter no. of switches:";
    L.Location=new Point(160,10); 
    L.AutoSize=true;
    

    t=new TextBox();
    t.Name="sw_no";
    t.Parent=this;
    t.Location=new Point(300,5); 
    t.Text="";

	Button b=new Button();
    b.Text="Submit";
    b.Location=new Point(410,5);  
    b.Parent=this;
    b.Click+=new EventHandler(Onsw_submit);
    
    cb1=new ComboBox();
    cb1.Parent=this;
    cb1.Text="Select switch";
    cb1.Location=new Point (10,47);
  

    cb3=new ComboBox();
    cb3.Parent=this;
    cb3.Text="Set DPID";
    cb3.Location=new Point (160,47);
    cb3.Items.AddRange(new object[]{"Auto"}); //add element to dropdown menu

    Label L2=new Label();
    L2.Parent=this;
    L2.Text="Enter no. of flows:";
    L2.Location=new Point(310,49);
    L2.AutoSize=true;

    t2=new TextBox();
    t2.Name="no_of_flows";
    t2.Parent=this;
    t2.Location=new Point(430,45);
    t2.Text="";

    Button b2=new Button();
    b2.Text="Submit";
    b2.Location=new Point(540,45);
    b2.Parent=this;
    b2.Click+=new EventHandler(Onfl_submit); 
    
    cb2=new ComboBox();
    cb2.Parent=this;
    cb2.Text="Select flow";
    cb2.Location=new Point (10,80);
    cb2.Size=new Size(620,100);

    Label L3=new Label();
    L3.Parent=this;
    L3.Text="Match";
    L3.Location=new Point(180,120);
    L3.AutoSize=true;

    Label L4=new Label();
    L4.Parent=this;
    L4.Text="Action";
    L4.Location=new Point(480,120);
    L4.AutoSize=true;

    Label L5=new Label();
    L5.Parent=this;
    L5.Text="stripvlan";
    L5.Location=new Point(360,590);
    L5.AutoSize=true;

    cb4=new ComboBox();
    cb4.Parent=this;
    cb4.Text="";
    cb4.Location=new Point (470,590);
    cb4.Items.AddRange(new object[]{"Yes","No"});

    Button b3=new Button();     
    b3.Text="Create Flow";
    b3.Location=new Point(350,630);
    b3.Size=new Size(330,30);
    b3.Parent=this;
    b3.Click+=new EventHandler(CreateFlow);

    Button b4=new Button();     
    b4.Text="Create Script";
    b4.Location=new Point(600,300);
    b4.Size=new Size(75,125);
    b4.Parent=this;
    b4.Click+=new EventHandler(CreateScript);

    Controls.Add(b); //add controls so that external funcs can handle the elements
    Controls.Add(t); 
    Controls.Add(t2);
    Controls.Add(cb1);
    Controls.Add(cb2);
    Controls.Add(cb3);
    Controls.Add(cb4);
   
}

//=======================**GUI section ends**===============================



void Onsw_submit(object sender,EventArgs e)
{                       //add elements to dropdown acc to no. of switches in txtbox
this.s=this.t.Text;     //assign text in textbox t to string s
for (int i = 0; i < Convert.ToInt32(s);i++)
    {
     string sn="switch"+Convert.ToString(i+1);  //e.g create names switch0, switch1
     this.cb1.Items.AddRange(new object[]{sn});
  
    }
}

void Onfl_submit(object sender,EventArgs e)
{                      //add elements to dropdown acc to no. of flows in txtbox t
this.cb2.Items.Clear();     //clear coombobox for flows of next flow
string s2=this.t2.Text;
for (int j = 0; j < Convert.ToInt32(s2);j++)
    {
     string sn2="Flow"+Convert.ToString(j);   //e.g create names flow0, flow1
     this.cb2.Items.AddRange(new object[]{sn2});
  
    }
}


void CreateFlow(object sender,EventArgs e)
{                     
    
using (StreamWriter target = new StreamWriter("temp.py",true)) //write all created flows to a temporary file
   
    {   
    
    string dpid="";    //for auto/manual dpid
    string sw_no="";
    string fl_no="";
    string sw_name=cb1.Text;
    string dpid_sel=cb3.Text;   
    
    for(int i=6;i<sw_name.Length;i++)    //extract sw no from name 
        {
            sw_no=sw_no+sw_name[i];
        }

    if (dpid_sel=="Auto")
    {
        long val=Convert.ToInt64(sw_no,16); //for auto dpid set dpid to sw_no+1, convert to hex & then to octal
        dpid=Convert.ToString(val,8);  
    }
    else 
    {
        long val=Convert.ToInt64(dpid_sel,16); //for manual dpid convert i/p to octal
        dpid=Convert.ToString(val,8);        
    }

    while(((dpid.Length)%3)!=0)
    {
        dpid="0"+dpid;     //arrange octal in grps of 3 for script
    }

   for(int i=4;i<cb2.Text.Length;i++)       //extract fl no from name
        {
             fl_no=fl_no+cb2.Text[i];
        }
string name ="flow"+sw_no+"_"+fl_no;            //create flow name to use in script e.g flow0_4
target.WriteLine("#"+name+" Match structure");
target.WriteLine(sw_name+"="+dpid);
target.WriteLine(name+"msg = of.ofp_flow_mod()"); 
target.WriteLine(name+"msg.cookie = 0 "); 


//=================================================
int go_mf=1;
int go_af=1;
int x,y;

foreach(var txt in me2)     //accessing the fields
{
  if(txt.Text!="")          //check if field is empty
  {
    x=go_mf;    //refer to switch case respective to field in textbox array
  
switch (x)
    {
    case 1:
        target.WriteLine(name+"msg.match.in_port ="+txt.Text);
        break;
    case 2:
        target.WriteLine(name+"msg.match.dl_type ="+txt.Text);
        break;
    case 3:
        target.WriteLine(name+"msg.match.nw_tos ="+txt.Text);
        break;
    case 4:
        target.WriteLine(name+"msg.match.nw_proto ="+txt.Text);
        break;
    case 5:
        target.WriteLine(name+"msg.match.nw_src = IPAddr(\""+txt.Text+"\")");
        break;
    case 6:
       target.WriteLine(name+"msg.match.nw_dst = IPAddr(\""+txt.Text+"\")");
        break;
    case 7:
      target.WriteLine(name+"msg.match.dl_vlan ="+txt.Text);
        break;
    case 8:
        target.WriteLine(name+"match.dl_vlan_pcp = "+txt.Text);
        break;    
    case 9:
        target.WriteLine(name+"msg.match.dl_src = EthAddr(\""+txt.Text+"\")");
        break;    
    case 10:
        target.WriteLine(name+"msg.match.dl_dst = EthAddr(\""+txt.Text+"\")");
        break;
    case 11:
        target.WriteLine(name+"msg.match.tp_src ="+txt.Text);
        break;
    case 12:
       target.WriteLine(name+"msg.match.tp_dst ="+txt.Text);
        break;
    case 13:
        target.WriteLine(name+"msg.priority ="+txt.Text);
        break;

    default:
        Console.WriteLine("");
        break;
    }
 }
 go_mf++;
}

List<string> action_list=new List<string>();

target.WriteLine("# ACTIONS----------------");

foreach(var txt in me3)
{

  if(txt.Text!="")
  {
    y=go_af;
  
switch (y)
    {
    case 1:
        target.WriteLine(name+"out = of.ofp_action_output (port = "+txt.Text+")");
        action_list.Add(name+"out");                        //add current actions name to action_list
        break;
    case 2:
        target.WriteLine(name+"vlanPriority = of.ofp_action_vlan_pcp (vlan_pcp ="+txt.Text+")");
        action_list.Add(name+"vlanPriority");
        break;
    case 3:
        target.WriteLine(name+"enqueue = of.ofp_action_enqueue (enqueue ="+txt.Text+")");
        action_list.Add(name+"enqueue");
        break;
    case 4:
        target.WriteLine(name+"srcPort = of.ofp_action_tp_port.set_src = (tp_port ="+txt.Text+")");
        action_list.Add(name+"srcPort");
        break;
    case 5:
        target.WriteLine(name+"dstPort = of.ofp_action_tp_port.set_dst = (tp_port = "+txt.Text+")");
        action_list.Add(name+"dstPort");
        break;
    case 6:
        target.WriteLine(name+"srcMAC = of.ofp_action_dl_addr.set_src(EthAddr(\""+txt.Text+"\"))");
        action_list.Add(name+"srcMAC");
        break;
    case 7:
        target.WriteLine(name+"dstMAC = of.ofp_action_dl_addr.set_dst(EthAddr(\""+txt.Text+"\"))");
        action_list.Add(name+"dstMAC");
        break;    
    case 8:
        target.WriteLine(name+"srcIP = of.ofp_action_nw_addr.set_src(IPAddr(\""+txt.Text+"\"))");
        action_list.Add(name+"srcIP");
        break;    
    case 9:
        target.WriteLine(name+"dstIP = of.ofp_action_nw_addr.set_dst(IPAddr(\""+txt.Text+"\"))");
        action_list.Add(name+"dstIP");
        break;
    case 10:
       target.WriteLine(name+"tos = of.ofp_action_nw_tos (nw_tos ="+txt.Text+")");
        action_list.Add(name+"tos");
        break;
    case 11:
       target.WriteLine(name+"vlan_id = of.ofp_action_vlan_vid (vlan_vid = "+txt.Text+")");
        action_list.Add(name+"vlan_id");
        break;
   
    default:
        Console.WriteLine("");
        break;
    }
 }
 go_af++;  //increment ctr
}


if(cb4.Text=="Yes")   //for stripvlan combobox
{
target.WriteLine(name+"stripvlan = of.ofp_action_strip_vlan()");
action_list.Add(name+"stripvlan");
}


//for action array
String[] action_list_array = action_list.ToArray(); //convert list to string array

target.WriteLine(name+"msg.actions = ["+string.Join(",",action_list_array)+"]"+"\n");

rec_sw.Add(cb1.Text);   
rec_fl.Add(name);
rec_count++;  //record total no. of flows created


//----------------------for clearing fields after pressing create flow
foreach(var txt in me2)     
{
 txt.Text=""; 
}

foreach(var txt in me3)     
{
  txt.Text="";
}
cb4.Text="";

}

}


//===================================================
void CreateScript(object sender,EventArgs e) 
{
    if(rec_count!=0)        //show success msg only if at least one flow is created
    {
    MessageBox.Show("Script created successfully", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
                    
 using (StreamWriter sw = new StreamWriter("controllerScript.py")) //open the file to write script into
    {   
        sw.WriteLine("\"\"\"\nScript created by POX custom flow generator (PCFG)\n\"\"\"\n");
        sw.WriteLine("from pox.core import core \nfrom pox.lib.addresses import IPAddr \nfrom pox.lib.addresses import EthAddr \nimport pox.openflow.libopenflow_01 as of");
        sw.WriteLine("\nlog = core.getLogger()\n");
        
        //open temp file in which flows are written & copy the contents to controllerScript.py
        using (StreamReader sr = new StreamReader("temp.py"))
        {
        string line;
        while ((line = sr.ReadLine()) != null)
            {
                sw.WriteLine(line);
            }   
        }

        
        sw.WriteLine("\ndef install_flows(): \n\tlog.info(\"  ### Installing static flows... ###\")\n");
        
        String[] rec_sw_str = rec_sw.ToArray();   //for sendtodpid msgs iterate through contents of rec_sw_str & rec_fl_str
        String[] rec_fl_str = rec_fl.ToArray();

        for(int u=0;u<rec_count;u++)
        {
          sw.WriteLine("\tcore.openflow.sendToDPID("+rec_sw_str[u]+","+rec_fl_str[u]+"msg)"); 
        }

        sw.WriteLine("\tlog.info(\"### Static flows installed. ###\")\n");
        sw.WriteLine("def launch (): \n\tlog.info(\"####Starting...####\")\n\tcore.callDelayed (15, install_flows)\n\tlog.info(\"### Waiting for switches to connect.. ###\")");
    
    }


File.Delete("temp.py"); //destroy temp file
Close();  //close window
}

//==============================================================================
	static public void Main()
	{
		Application.Run(new foo()); //run the app

	}
}
