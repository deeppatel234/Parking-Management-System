using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using System.Windows.Threading;
using System.Data.SqlClient;
using System.Configuration;

namespace DEFinal
{
   
    public partial class MainWindow : Window
    {
        DAO dao = new DAO();
        SerialPort serialPort = new SerialPort();
        SqlConnection conn = DAO.conn;


        int total = 0;
        int sensor1count = 0;
        int sensor2count = 0;
        int sensor3count = 0;
        int sensor4count = 0;
        string[] sort = new string[4];
        string[] abc;

        int CarCount = 0;
        int FullCount = 4;

        int MaintenanceCount = 0;


        string PDFGateName = "ALL";
        string PDFCar = "ALLCAR";

        public MainWindow()
        {
            InitializeComponent();

            serialPort.BaudRate = 9600;
            serialPort.PortName = "COM4";
            serialPort.DataReceived += SerialPort1_DataReceived;

            try
            {
                serialPort.Open();
                AvailibleTxt.Text = "System is Online (Available)";
                AvailibleTxt.Foreground = new SolidColorBrush(Colors.Green);
                CommaentTxt.Text = "Cars : "+ CarCount + "   |   Total Capacity : "+ FullCount + "   |   Maintenance Sloats : " + MaintenanceCount;
            }
            catch
            {
               // MessageBox.Show("Error in connecting to the system");
            }

            dao.opeConnection();

            SqlCommand com = new SqlCommand("select * from usagedata", conn);

            SqlDataReader dr = com.ExecuteReader();

            while (dr.Read())
            {
                sensor1count = dr.GetInt32(1);
                sensor2count = dr.GetInt32(2);
                sensor3count = dr.GetInt32(3);
                sensor4count = dr.GetInt32(4);
                total = dr.GetInt32(5);
            }
            dr.Close();
            
            printvalue();
        }

        private delegate void UpdateUiTextDelegate(string text);
        string RxString;
        private void SerialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            RxString = serialPort.ReadLine();
            Dispatcher.Invoke(DispatcherPriority.Send, new UpdateUiTextDelegate(DisplayText), RxString);
        }



        string entry_gatetype, entry_gateopentime, entry_gateclosetime, entry_gateopentype;
        string exit_gatetype, exit_gateopentime, exit_gateclosetime, exit_gateopentype;

        string slot1_starttime, slot1_endtime;
        string slot2_starttime, slot2_endtime;
        string slot3_starttime, slot3_endtime;
        string slot4_starttime, slot4_endtime;

        private void DisplayText(string rxstring)
        {

            switch (RxString)
            {
                case "Sensor1-OFF\r":
                    slot1_endtime = DateTime.Now.ToString();
                    dao.addSlotusage(1, slot1_starttime, slot1_endtime);
                    Sensor1DockPanel.Background = new SolidColorBrush(Colors.Green);
                    break;

                case "Sensor1-ON\r":
                    sensor1count++;
                    total++;
                    slot1_starttime = DateTime.Now.ToString();
                    Sensor1DockPanel.Background = new SolidColorBrush(Colors.Red);
                    break;

                case "Sensor2-OFF\r":
                    slot2_endtime = DateTime.Now.ToString();
                    dao.addSlotusage(2, slot2_starttime, slot2_endtime);
                    Sensor2DockPanel.Background = new SolidColorBrush(Colors.Green);
                    break;

                case "Sensor2-ON\r":
                    sensor2count++;
                    total++;
                    slot2_starttime = DateTime.Now.ToString();
                    Sensor2DockPanel.Background = new SolidColorBrush(Colors.Red);
                    break;


                case "Sensor3-OFF\r":
                    slot3_endtime = DateTime.Now.ToString();
                    dao.addSlotusage(3, slot3_starttime, slot3_endtime);
                    Sensor3DockPanel.Background = new SolidColorBrush(Colors.Green);
                    break;

                case "Sensor3-ON\r":
                    sensor3count++;
                    total++;
                    slot3_starttime = DateTime.Now.ToString();
                    Sensor3DockPanel.Background = new SolidColorBrush(Colors.Red);
                    break;

                case "Sensor4-OFF\r":
                    slot4_endtime = DateTime.Now.ToString();
                    dao.addSlotusage(4, slot4_starttime, slot4_endtime);
                    Sensor4DockPanel.Background = new SolidColorBrush(Colors.Green);
                    break;
                case "Sensor4-ON\r":
                    sensor4count++;
                    total++;
                    slot4_starttime = DateTime.Now.ToString();
                    Sensor4DockPanel.Background = new SolidColorBrush(Colors.Red);
                    break;

                case "EntryGateOpen-ON\r":
                    entry_gatetype = "Entry Gate";
                    entry_gateopentype = "Automatic";
                    entry_gateopentime = DateTime.Now.ToString();
                    EntryGateImage.Source = new BitmapImage(new Uri(@"/Images/opengate.png", UriKind.Relative));
                    break;
                case "EntryGateClosed-ON\r":
                    entry_gateclosetime = DateTime.Now.ToString();
                    dao.addGatedata(entry_gatetype, entry_gateopentime, entry_gateclosetime, entry_gateopentype);
                    EntryGateImage.Source = new BitmapImage(new Uri(@"/Images/Closegate.png", UriKind.Relative));
                    break;

                case "ExitGateOpen-ON\r":
                    exit_gatetype = "Exit Gate";
                    exit_gateopentype = "Automatic";
                    exit_gateopentime = DateTime.Now.ToString();
                    ExitGateImage.Source = new BitmapImage(new Uri(@"/Images/opengate.png", UriKind.Relative));
                    break;
                case "ExitGateClosed-ON\r":
                    exit_gateclosetime = DateTime.Now.ToString();
                    dao.addGatedata(exit_gatetype, exit_gateopentime, exit_gateclosetime, exit_gateopentype);
                    ExitGateImage.Source = new BitmapImage(new Uri(@"/Images/Closegate.png", UriKind.Relative));
                    break;

                default:
                    if(RxString.StartsWith("CarCount"))
                    {
                        string[] coun = RxString.Split('-');
                        CarCount = Convert.ToInt32(coun[1]);
                    }

                    if(RxString.StartsWith("FULLCount"))
                    {
                        string[] coun = RxString.Split('-');
                        FullCount = Convert.ToInt32(coun[1]);
                    }
                    break;

            }

            printvalue();
        }

        private void printvalue()
        {
            Sensor1Text.Text = "Slot 1 Usage : " + sensor1count;
            Sensor2Text.Text = "Slot 2 Usage : " + sensor2count;
            Sensor3Text.Text = "Slot 3 Usage : " + sensor3count;
            Sensor4Text.Text = "Slot 4 Usage : " + sensor4count;
            TotalUsageText.Text = "Total Usage of Parking Slots : " + total;

            sort[0] = "1 + " + sensor1count;
            sort[1] = "2 + " + sensor2count;
            sort[2] = "3 + " + sensor3count;
            sort[3] = "4 + " + sensor4count;

            sort = Sorting.Top(sort);

            abc = sort[0].Split('+');
            Top1UsageTxt.Text = "Top Slot " + abc[0] + " Usage : " + abc[1];
            abc = sort[1].Split('+');
            Top2UsageTxt.Text = "Top Slot " + abc[0] + " Usage : " + abc[1];
            Bottom3UsageTxt.Text = "Bottom Slot " + abc[0] + " Usage: " + abc[1];
            abc = sort[2].Split('+');
            Top3UsageTxt.Text = "Top Slot " + abc[0] + " Usage : " + abc[1];
            Bottom2UsageTxt.Text = "Bottom Slot " + abc[0] + " Usage: " + abc[1];

            abc = sort[3].Split('+');
            Bottom1UsageTxt.Text = "Bottom Slot " + abc[0] + " Usage: " + abc[1];

            GateCountTxt.Text = "Gate Count : "+CarCount+" / " + FullCount;

            CommaentTxt.Text = "Cars : " + CarCount + "   |   Total Capacity : " + FullCount + "   |   Maintenance Sloats : " + MaintenanceCount;

            if (CarCount == FullCount)
            {
                FullGateImage.Source = new BitmapImage(new Uri(@"/Images/full.png", UriKind.Relative));
            }
            else
            {
                FullGateImage.Source = new BitmapImage(new Uri(@"/Images/opengate.png", UriKind.Relative));
            }
        }

        int sloat1maintanacetemp = 2;
        private void Sloat1Maintanace_Click(object sender, RoutedEventArgs e)
        {
            if (sloat1maintanacetemp % 2 == 0)
            {
                serialPort.Write("S10");
                Sloat1Maintanace.Content = "Under Maintenance";
                Sloat1Maintanace.Background = new SolidColorBrush(Colors.Red);
                Sloat1Maintanace.FontSize = 20;

                Sensor1DockPanel.Background = new SolidColorBrush(Colors.Red);
                StatusSensor1Txt.Text = "Not Available";
                StatusSensor1Txt.FontSize = 25;
                MaintenanceCount++;

                slot1_starttime = DateTime.Now.ToString();
            }
            else
            {
                serialPort.Write("S11");
                Sloat1Maintanace.Content = "Sloat 1 OPEN";
                Sloat1Maintanace.Background = new SolidColorBrush(Colors.Green);
                Sloat1Maintanace.FontSize = 30;

                Sensor1DockPanel.Background = new SolidColorBrush(Colors.Green);
                StatusSensor1Txt.Text = "Slot 1";
                StatusSensor1Txt.FontSize = 40;
                MaintenanceCount--;

                slot1_endtime = DateTime.Now.ToString();
                dao.addMaintenanceslotusage(1, slot1_starttime, slot1_endtime);
            }
            sloat1maintanacetemp++;
        }

        int sloat2maintanacetemp = 2;

        

        private void Sloat2Maintanace_Click(object sender, RoutedEventArgs e)
        {
            if (sloat2maintanacetemp % 2 == 0)
            {
                serialPort.Write("S20");
                Sloat2Maintanace.Content = "Under maintenance";
                Sloat2Maintanace.Background = new SolidColorBrush(Colors.Red);
                Sloat2Maintanace.FontSize = 20;

                Sensor2DockPanel.Background = new SolidColorBrush(Colors.Red);
                StatusSensor2Txt.Text = "Not Available";
                StatusSensor2Txt.FontSize = 25;
                MaintenanceCount++;

                slot2_starttime = DateTime.Now.ToString();
            }
            else
            {
                serialPort.Write("S21");
                Sloat2Maintanace.Content = "Sloat 2 OPEN";
                Sloat2Maintanace.Background = new SolidColorBrush(Colors.Green);
                Sloat2Maintanace.FontSize = 30;

                Sensor2DockPanel.Background = new SolidColorBrush(Colors.Green);
                StatusSensor2Txt.Text = "Slot 2";
                StatusSensor2Txt.FontSize = 40;
                MaintenanceCount--;

                slot2_endtime = DateTime.Now.ToString();
                dao.addMaintenanceslotusage(2, slot2_starttime, slot2_endtime);
            }
            sloat2maintanacetemp++;
        }

        int sloat3maintanacetemp = 2;

       
        private void Sloat3Maintanace_Click_1(object sender, RoutedEventArgs e)
        {
            if (sloat3maintanacetemp % 2 == 0)
            {
                serialPort.Write("S30");
                Sloat3Maintanace.Content = "Under maintenance";
                Sloat3Maintanace.Background = new SolidColorBrush(Colors.Red);
                Sloat3Maintanace.FontSize = 20;

                Sensor3DockPanel.Background = new SolidColorBrush(Colors.Red);
                StatusSensor3Txt.Text = "Not Available";
                StatusSensor3Txt.FontSize = 25;
                MaintenanceCount++;

                slot3_starttime = DateTime.Now.ToString();
            }
            else
            {
                serialPort.Write("S31");
                Sloat3Maintanace.Content = "Sloat 3 OPEN";
                Sloat3Maintanace.Background = new SolidColorBrush(Colors.Green);
                Sloat3Maintanace.FontSize = 30;

                Sensor3DockPanel.Background = new SolidColorBrush(Colors.Green);
                StatusSensor3Txt.Text = "Slot 3";
                StatusSensor3Txt.FontSize = 40;
                MaintenanceCount--;

                slot3_endtime = DateTime.Now.ToString();
                dao.addMaintenanceslotusage(3, slot3_starttime, slot3_endtime);
            }
            sloat3maintanacetemp++;
        }

        int sloat4maintanacetemp = 2;

        

        private void Sloat4Maintanace_Click(object sender, RoutedEventArgs e)
        {
            if (sloat4maintanacetemp % 2 == 0)
            {
                serialPort.Write("S40");
                Sloat4Maintanace.Content = "Under maintenance";
                Sloat4Maintanace.Background = new SolidColorBrush(Colors.Red);
                Sloat4Maintanace.FontSize = 20;

                Sensor4DockPanel.Background = new SolidColorBrush(Colors.Red);
                StatusSensor4Txt.Text = "Not Available";
                StatusSensor4Txt.FontSize = 25;
                MaintenanceCount++;

                slot4_starttime = DateTime.Now.ToString();
            }
            else
            {
                serialPort.Write("S41");
                Sloat4Maintanace.Content = "Sloat 4 OPEN";
                Sloat4Maintanace.Background = new SolidColorBrush(Colors.Green);
                Sloat4Maintanace.FontSize = 30;

                Sensor4DockPanel.Background = new SolidColorBrush(Colors.Green);
                StatusSensor4Txt.Text = "Slot 4";
                StatusSensor4Txt.FontSize = 40;
                MaintenanceCount--;

                slot4_endtime = DateTime.Now.ToString();
                dao.addMaintenanceslotusage(4, slot4_starttime, slot4_endtime);
            }
            sloat4maintanacetemp++;
        }
        
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(total != sensor1count+sensor2count+sensor3count+sensor4count)
            {
                total = sensor1count + sensor2count + sensor3count + sensor4count;
            }

            SqlCommand com = new SqlCommand("update usagedata set firstsloat=@firstsloat , secoundsloat=@secoundsloat , thirdsloat = @thirdsloat , fourthsloat = @fourthsloat , total = @total where Id=1", conn);
            com.Parameters.AddWithValue("firstsloat", sensor1count);
            com.Parameters.AddWithValue("secoundsloat", sensor2count);
            com.Parameters.AddWithValue("thirdsloat", sensor3count);
            com.Parameters.AddWithValue("fourthsloat", sensor4count);
            com.Parameters.AddWithValue("total", total);
            
            com.ExecuteNonQuery();

            dao.closeConnection();
        }

        private void EntryGateOpenBtn_Click(object sender, RoutedEventArgs e)
        {
            serialPort.Write("G10");
            EntryGateImage.Source = new BitmapImage(new Uri(@"/Images/opengate.png", UriKind.Relative));
            entry_gatetype = "Entry Gate";
            entry_gateopentype = "Manual";
            entry_gateopentime = DateTime.Now.ToString();
        }

        int usageDetailsBtnTemp = 2;

        private void UsageDetailBtn_Click(object sender, RoutedEventArgs e)
        {
            if (usageDetailsBtnTemp % 2 == 0)
            {
                UsageReportGrid.Width = Double.NaN;
                printUsageData(dao.getAllCarUsageLog());
                UsageDetailsArrowimg.Source = new BitmapImage(new Uri(@"/Images/leftarrow.png", UriKind.Relative));
                PDFCar = "ALLCAR";
            }
            else
            {
                UsageReportGrid.Width = 0;
                UsageDetailsArrowimg.Source = new BitmapImage(new Uri(@"/Images/rightarrow.png", UriKind.Relative));
            }

            usageDetailsBtnTemp++;
        }

        int GateDetailsTemp = 2;
        private void GateDetailBtn_Click(object sender, RoutedEventArgs e)
        {
            if (GateDetailsTemp % 2 == 0)
            {
                GateSatusArrowimg.Source = new BitmapImage(new Uri(@"/Images/rightarrow.png", UriKind.Relative));
                GateStatusGrid.Width = Double.NaN;
                uGrid.Width = new GridLength(0);
                DashbordGrid.Width = new GridLength(0);
                printGateUsageData(dao.getAllGateUsageLog());
                PDFGateName = "ALL";
            }
            else
            {
                GateStatusGrid.Width = 0;
                GateSatusArrowimg.Source = new BitmapImage(new Uri(@"/Images/leftarrow.png", UriKind.Relative));
                uGrid.Width = new GridLength(320);
                DashbordGrid.Width = new GridLength(1, GridUnitType.Star);
            }

            GateDetailsTemp++;
        }

        

        private void ExitGateClose_Click(object sender, RoutedEventArgs e)
        {
            serialPort.Write("G11");
            EntryGateImage.Source = new BitmapImage(new Uri(@"/Images/Closegate.png", UriKind.Relative));
            entry_gateclosetime = DateTime.Now.ToString();
            dao.addGatedata(entry_gatetype, entry_gateopentime, entry_gateclosetime, entry_gateopentype);
        }

        private void ExitGateOpen_Click(object sender, RoutedEventArgs e)
        {
            serialPort.Write("G20");
            exit_gatetype = "Exit Gate";
            exit_gateopentype = "Manual";
            exit_gateopentime = DateTime.Now.ToString();
            ExitGateImage.Source = new BitmapImage(new Uri(@"/Images/opengate.png", UriKind.Relative));
        }

        

        private void ExitGateCloseBtn_Click(object sender, RoutedEventArgs e)
        {
            serialPort.Write("G21");
            ExitGateImage.Source = new BitmapImage(new Uri(@"/Images/Closegate.png", UriKind.Relative));
            exit_gateclosetime = DateTime.Now.ToString();
            dao.addGatedata(exit_gatetype, exit_gateopentime, exit_gateclosetime, exit_gateopentype);
        }

        

        int UsageTemp = 1;
        private void CarUsageDetailsBtn_Click(object sender, RoutedEventArgs e)
        {
            carusagepoly.Visibility = Visibility.Visible;
            maintenanceusagepoly.Visibility = Visibility.Hidden;
            UsageLabelTxt.Text = "Car Usage Details";
            printUsageData(dao.getAllCarUsageLog());
            UsageTemp = 1;
            PDFCar = "ALLCAR";
        }

        

        private void MainteinanceUsageBtn_Click(object sender, RoutedEventArgs e)
        {
            carusagepoly.Visibility = Visibility.Hidden;
            maintenanceusagepoly.Visibility = Visibility.Visible;
            printUsageData(dao.getAllMaintenanceLog());
            UsageLabelTxt.Text = "Maintenance Usage Details";
            UsageTemp = 2;
            PDFCar = "ALLMaintenance";
        }

        

        private void printUsageData(SqlDataReader dr)
        {
            MaintenanceSlotnoStack.Children.Clear();
            MaintenanceStarttimeStack.Children.Clear();
            MaintenanceEndtimeStack.Children.Clear();
            
            TextBlock tb;

            tb = new TextBlock();
            tb.Text = "Slot No.";
            tb.FontSize = 18;
            tb.Height = 25;
            tb.Background = new SolidColorBrush(Colors.Transparent);
            tb.Foreground = new SolidColorBrush(Colors.White);
            tb.FontWeight = FontWeights.Bold;
            MaintenanceSlotnoStack.Children.Add(tb);

            tb = new TextBlock();
            tb.Text = "Start Date and Time";
            tb.FontSize = 18;
            tb.Height = 25;
            tb.Background = new SolidColorBrush(Colors.Transparent);
            tb.Foreground = new SolidColorBrush(Colors.White);
            tb.FontWeight = FontWeights.Bold;
            MaintenanceStarttimeStack.Children.Add(tb);

            tb = new TextBlock();
            tb.Text = "End Date and Time";
            tb.FontSize = 18;
            tb.Height = 25;
            tb.Background = new SolidColorBrush(Colors.Transparent);
            tb.Foreground = new SolidColorBrush(Colors.White);
            tb.FontWeight = FontWeights.Bold;
            MaintenanceEndtimeStack.Children.Add(tb);

            while (dr.Read())
            {
                tb = new TextBlock();
                tb.Text = dr.GetValue(1).ToString();
                tb.FontSize = 15;
                tb.Height = 20;
                tb.Background = new SolidColorBrush(Colors.Transparent);
                tb.Foreground = new SolidColorBrush(Colors.White);

                MaintenanceSlotnoStack.Children.Add(tb);

                tb = new TextBlock();
                tb.Text = dr.GetValue(2).ToString();
                tb.FontSize = 15;
                tb.Height = 20;
                tb.Background = new SolidColorBrush(Colors.Transparent);
                tb.Foreground = new SolidColorBrush(Colors.White);

                MaintenanceStarttimeStack.Children.Add(tb);

                tb = new TextBlock();
                tb.Text = dr.GetValue(3).ToString();
                tb.FontSize = 15;
                tb.Height = 20;
                tb.Background = new SolidColorBrush(Colors.Transparent);
                tb.Foreground = new SolidColorBrush(Colors.White);

                MaintenanceEndtimeStack.Children.Add(tb);
            }

            dr.Close();
        }

        

        private void SearchbySloatMaintenanceBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (UsageTemp == 2)
                {
                    printUsageData(dao.getAllMaintenanceLogbySlotNo(Convert.ToInt32(((ComboBoxItem)Maintenancesloatusagecombo.SelectedItem).Content.ToString())));
                    PDFCar = "MaintenanceSlot";
                }

                if (UsageTemp == 1)
                {
                    PDFCar = "CARSlot";
                    printUsageData(dao.getAllCarUsageLogbySlotNo(Convert.ToInt32(((ComboBoxItem)Maintenancesloatusagecombo.SelectedItem).Content.ToString())));
                    
                }
            }
            catch
            {
                MessageBox.Show("Select Slot First");
            }
        }



        private void maintanaceusageresetBtn_Click(object sender, RoutedEventArgs e)
        {
            if (UsageTemp == 2)
            {
                printUsageData(dao.getAllMaintenanceLog());
                PDFCar = "ALLMaintenance";
            }
            if (UsageTemp == 1)
            {
                printUsageData(dao.getAllCarUsageLog());
                PDFCar = "ALLCAR";
            }
        }

        private void SearchbyDateMaintenanceBtn_Click(object sender, RoutedEventArgs e)
        {
            string startDate = startdatemaintenancedate.SelectedDate.ToString();
            string endDate = enddatemaintenancedate.SelectedDate.ToString();

            MaintenanceSlotnoStack.Children.Clear();
            MaintenanceStarttimeStack.Children.Clear();
            MaintenanceEndtimeStack.Children.Clear();

            SqlDataReader dr = null;
            if (UsageTemp == 2)
            {
                dr = dao.getAllMaintenanceLog();
                PDFCar = "MaintenanceDate";
            }

            if (UsageTemp == 1)
            { 
                dr = dao.getAllCarUsageLog();
                PDFCar = "CARDate";
            }

           
            TextBlock tb;

            tb = new TextBlock();
            tb.Text = "Slot No.";
            tb.FontSize = 18;
            tb.Height = 25;
            tb.Background = new SolidColorBrush(Colors.Transparent);
            tb.Foreground = new SolidColorBrush(Colors.White);
            tb.FontWeight = FontWeights.Bold;
            MaintenanceSlotnoStack.Children.Add(tb);

            tb = new TextBlock();
            tb.Text = "Start Date and Time";
            tb.FontSize = 18;
            tb.Height = 25;
            tb.Background = new SolidColorBrush(Colors.Transparent);
            tb.Foreground = new SolidColorBrush(Colors.White);
            tb.FontWeight = FontWeights.Bold;
            MaintenanceStarttimeStack.Children.Add(tb);

            tb = new TextBlock();
            tb.Text = "End Date and Time";
            tb.FontSize = 18;
            tb.Height = 25;
            tb.Background = new SolidColorBrush(Colors.Transparent);
            tb.Foreground = new SolidColorBrush(Colors.White);
            tb.FontWeight = FontWeights.Bold;
            MaintenanceEndtimeStack.Children.Add(tb);

            string[] splitStartDate;
            string[] splitEndDate;

            while (dr.Read())
            {
                splitStartDate = dr.GetValue(2).ToString().Split(' ');
                splitEndDate = dr.GetValue(3).ToString().Split(' ');

                if (DateTime.Parse(splitStartDate[0]) >= DateTime.Parse(startDate) && DateTime.Parse(splitEndDate[0]) <= DateTime.Parse(endDate))
                {
                    tb = new TextBlock();
                    tb.Text = dr.GetValue(1).ToString();
                    tb.FontSize = 15;
                    tb.Height = 20;
                    tb.Background = new SolidColorBrush(Colors.Transparent);
                    tb.Foreground = new SolidColorBrush(Colors.White);

                    MaintenanceSlotnoStack.Children.Add(tb);

                    tb = new TextBlock();
                    tb.Text = dr.GetValue(2).ToString();
                    tb.FontSize = 15;
                    tb.Height = 20;
                    tb.Background = new SolidColorBrush(Colors.Transparent);
                    tb.Foreground = new SolidColorBrush(Colors.White);

                    MaintenanceStarttimeStack.Children.Add(tb);

                    tb = new TextBlock();
                    tb.Text = dr.GetValue(3).ToString();
                    tb.FontSize = 15;
                    tb.Height = 20;
                    tb.Background = new SolidColorBrush(Colors.Transparent);
                    tb.Foreground = new SolidColorBrush(Colors.White);

                    MaintenanceEndtimeStack.Children.Add(tb);
                }
            }

            dr.Close();
        }


        private void printGateUsageData(SqlDataReader dr)
        {
            gatetypeStack.Children.Clear();
            gateStarttimeStack.Children.Clear();
            gateEndtimeStack.Children.Clear();
            gateopentypeStack.Children.Clear();

            TextBlock tb;

            tb = new TextBlock();
            tb.Text = "Gate Type";
            tb.FontSize = 18;
            tb.Height = 25;
            tb.Background = new SolidColorBrush(Colors.Transparent);
            tb.Foreground = new SolidColorBrush(Colors.White);
            tb.FontWeight = FontWeights.Bold;
            gatetypeStack.Children.Add(tb);

            tb = new TextBlock();
            tb.Text = "Open Date and Time";
            tb.FontSize = 18;
            tb.Height = 25;
            tb.Background = new SolidColorBrush(Colors.Transparent);
            tb.Foreground = new SolidColorBrush(Colors.White);
            tb.FontWeight = FontWeights.Bold;
            gateStarttimeStack.Children.Add(tb);

            tb = new TextBlock();
            tb.Text = "Close Date and Time";
            tb.FontSize = 18;
            tb.Height = 25;
            tb.Background = new SolidColorBrush(Colors.Transparent);
            tb.Foreground = new SolidColorBrush(Colors.White);
            tb.FontWeight = FontWeights.Bold;
            gateEndtimeStack.Children.Add(tb);

            tb = new TextBlock();
            tb.Text = "Mode";
            tb.FontSize = 18;
            tb.Height = 25;
            tb.Background = new SolidColorBrush(Colors.Transparent);
            tb.Foreground = new SolidColorBrush(Colors.White);
            tb.FontWeight = FontWeights.Bold;
            gateopentypeStack.Children.Add(tb);

            while (dr.Read())
            {
                tb = new TextBlock();
                tb.Text = dr.GetValue(1).ToString();
                tb.FontSize = 15;
                tb.Height = 20;
                tb.Background = new SolidColorBrush(Colors.Transparent);
                tb.Foreground = new SolidColorBrush(Colors.White);

                gatetypeStack.Children.Add(tb);

                tb = new TextBlock();
                tb.Text = dr.GetValue(2).ToString();
                tb.FontSize = 15;
                tb.Height = 20;
                tb.Background = new SolidColorBrush(Colors.Transparent);
                tb.Foreground = new SolidColorBrush(Colors.White);

                gateStarttimeStack.Children.Add(tb);

                tb = new TextBlock();
                tb.Text = dr.GetValue(3).ToString();
                tb.FontSize = 15;
                tb.Height = 20;
                tb.Background = new SolidColorBrush(Colors.Transparent);
                tb.Foreground = new SolidColorBrush(Colors.White);

                gateEndtimeStack.Children.Add(tb);

                tb = new TextBlock();
                tb.Text = dr.GetValue(4).ToString();
                tb.FontSize = 15;
                tb.Height = 20;
                tb.Background = new SolidColorBrush(Colors.Transparent);
                tb.Foreground = new SolidColorBrush(Colors.White);
                gateopentypeStack.Children.Add(tb);
            }

            dr.Close();
        }


        private void GateUsageDetailsBtn_Click(object sender, RoutedEventArgs e)
        {
            printGateUsageData(dao.getAllGateUsageLog());
            PDFGateName = "ALL";
        }

        private void gateusageresetBtn_Click(object sender, RoutedEventArgs e)
        {
            printGateUsageData(dao.getAllGateUsageLog());
            PDFGateName = "ALL";
        }

        private void SearchbygatetypeBtn_Click(object sender, RoutedEventArgs e)
        {
           printGateUsageData(dao.getGateUsagebytype( ((ComboBoxItem)gatetypeloatusagecombo.SelectedItem).Content.ToString()));
            PDFGateName = "GateType";
        }

        
        private void SearchbyDategateBtn_Click(object sender, RoutedEventArgs e)
        {
            PDFGateName = "GateDates";
            string startDate = startdategatedate.SelectedDate.ToString();
            string endDate = enddategatedate.SelectedDate.ToString();

            gatetypeStack.Children.Clear();
            gateStarttimeStack.Children.Clear();
            gateEndtimeStack.Children.Clear();
            gateopentypeStack.Children.Clear();

            SqlDataReader dr = dao.getAllGateUsageLog();

            TextBlock tb;

            tb = new TextBlock();
            tb.Text = "Gate Type";
            tb.FontSize = 18;
            tb.Height = 25;
            tb.Background = new SolidColorBrush(Colors.Transparent);
            tb.Foreground = new SolidColorBrush(Colors.White);
            tb.FontWeight = FontWeights.Bold;
            gatetypeStack.Children.Add(tb);

            tb = new TextBlock();
            tb.Text = "Open Date and Time";
            tb.FontSize = 18;
            tb.Height = 25;
            tb.Background = new SolidColorBrush(Colors.Transparent);
            tb.Foreground = new SolidColorBrush(Colors.White);
            tb.FontWeight = FontWeights.Bold;
            gateStarttimeStack.Children.Add(tb);

            tb = new TextBlock();
            tb.Text = "Close Date and Time";
            tb.FontSize = 18;
            tb.Height = 25;
            tb.Background = new SolidColorBrush(Colors.Transparent);
            tb.Foreground = new SolidColorBrush(Colors.White);
            tb.FontWeight = FontWeights.Bold;
            gateEndtimeStack.Children.Add(tb);

            tb = new TextBlock();
            tb.Text = "Mode";
            tb.FontSize = 18;
            tb.Height = 25;
            tb.Background = new SolidColorBrush(Colors.Transparent);
            tb.Foreground = new SolidColorBrush(Colors.White);
            tb.FontWeight = FontWeights.Bold;
            gateopentypeStack.Children.Add(tb);

            string[] splitStartDate;
            string[] splitEndDate;

            while (dr.Read())
            {
                splitStartDate = dr.GetValue(2).ToString().Split(' ');
                splitEndDate = dr.GetValue(3).ToString().Split(' ');

                if (DateTime.Parse(splitStartDate[0]) >= DateTime.Parse(startDate) && DateTime.Parse(splitEndDate[0]) <= DateTime.Parse(endDate))
                {

                    tb = new TextBlock();
                    tb.Text = dr.GetValue(1).ToString();
                    tb.FontSize = 15;
                    tb.Height = 20;
                    tb.Background = new SolidColorBrush(Colors.Transparent);
                    tb.Foreground = new SolidColorBrush(Colors.White);

                    gatetypeStack.Children.Add(tb);

                    tb = new TextBlock();
                    tb.Text = dr.GetValue(2).ToString();
                    tb.FontSize = 15;
                    tb.Height = 20;
                    tb.Background = new SolidColorBrush(Colors.Transparent);
                    tb.Foreground = new SolidColorBrush(Colors.White);

                    gateStarttimeStack.Children.Add(tb);

                    tb = new TextBlock();
                    tb.Text = dr.GetValue(3).ToString();
                    tb.FontSize = 15;
                    tb.Height = 20;
                    tb.Background = new SolidColorBrush(Colors.Transparent);
                    tb.Foreground = new SolidColorBrush(Colors.White);

                    gateEndtimeStack.Children.Add(tb);

                    tb = new TextBlock();
                    tb.Text = dr.GetValue(4).ToString();
                    tb.FontSize = 15;
                    tb.Height = 20;
                    tb.Background = new SolidColorBrush(Colors.Transparent);
                    tb.Foreground = new SolidColorBrush(Colors.White);
                    gateopentypeStack.Children.Add(tb);
                }
            }

            dr.Close();
        }

        private void SearchbygateopentypeBtn_Click(object sender, RoutedEventArgs e)
        {
            printGateUsageData(dao.getGateUsagebyopentype(((ComboBoxItem)gateopentypeloatusagecombo.SelectedItem).Content.ToString()));
            PDFGateName = "MODE";
        }




        PDF pdf = new PDF();
        private void getPDFBtn_Click(object sender, RoutedEventArgs e)
        {
            if (PDFGateName == "ALL")
            {
                pdf.getAllGateDetails(dao.getAllGateUsageLog());
            }
            else if(PDFGateName == "GateType")
            {
                pdf.printGateData(dao.getGateUsagebytype(((ComboBoxItem)gatetypeloatusagecombo.SelectedItem).Content.ToString()));
            }
            else if (PDFGateName == "MODE")
            {
                pdf.printGateData((dao.getGateUsagebyopentype(((ComboBoxItem)gateopentypeloatusagecombo.SelectedItem).Content.ToString())));
            }
            else if (PDFGateName == "GateDates")
            {
                string startDate1 = startdategatedate.SelectedDate.ToString();
                string endDate1 = enddategatedate.SelectedDate.ToString();
                pdf.getbyDate(startDate1, endDate1,dao.getAllGateUsageLog());
            }
        }


        private void carPDFBtn_Click(object sender, RoutedEventArgs e)
        {
            if(PDFCar == "ALLCAR")
            {
                pdf.printCarUsageData(dao.getAllCarUsageLog(),"Car Slot ");
            }
            else if(PDFCar == "ALLMaintenance")
            {
                pdf.printCarUsageData(dao.getAllMaintenanceLog(),"Maintenance");
            }
            else if(PDFCar == "CARDate")
            {
                string startDate3 = startdatemaintenancedate.SelectedDate.ToString();
                string endDate3 = enddatemaintenancedate.SelectedDate.ToString();
                pdf.printCarDateDetails(dao.getAllCarUsageLog(),"Car Slot ",startDate3,endDate3);
            }
            else if (PDFCar == "MaintenanceDate")
            {
                string startDate4 = startdatemaintenancedate.SelectedDate.ToString();
                string endDate4 = enddatemaintenancedate.SelectedDate.ToString();
                pdf.printCarDateDetails(dao.getAllMaintenanceLog(), "Car Slot ", startDate4, endDate4);
            }
            else if (PDFCar == "CARSlot")
            {
                pdf.printCarUsageData(dao.getAllCarUsageLogbySlotNo(Convert.ToInt32(((ComboBoxItem)Maintenancesloatusagecombo.SelectedItem).Content.ToString())), "Car Slot ");
            }
            else if (PDFCar == "MaintenanceSlot")
            {
                pdf.printCarUsageData(dao.getAllMaintenanceLogbySlotNo(Convert.ToInt32(((ComboBoxItem)Maintenancesloatusagecombo.SelectedItem).Content.ToString())),"Maintenance ");
            }
        }
    }
}



public class Sorting
{
    public static string[] Top(string[] sort)
    {
        int[] sensorIndex = new int[4];
        int[] sensorValues = new int[4];
        String[] temp;

        for (int j = 0; j < 4; j++)
        {
            temp = sort[j].Split('+');
            sensorIndex[j] = Convert.ToInt32(temp[0]);
            sensorValues[j] = Convert.ToInt32(temp[1]);
        }
       
        for(int i = 0; i < 4; i++)
        {
            for (int p = 0; p < 4; p++)
            {
                if (sensorValues[i] > sensorValues[p])
                {
                    int t = sensorValues[i];
                    sensorValues[i] = sensorValues[p];
                    sensorValues[p] = t;

                    t = sensorIndex[i];
                    sensorIndex[i] = sensorIndex[p];
                    sensorIndex[p] = t;
                }
            }
        }

        for (int k = 0; k < 4; k++)
        {
            sort[k] = sensorIndex[k] + "+" + sensorValues[k];
        }

        return sort;
    }
}