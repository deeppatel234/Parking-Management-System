using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEFinal
{
    class DAO
    {
        public static SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["DBURL"].ConnectionString);
        public void opeConnection()
        {
            conn.Open();
        }
        public void closeConnection()
        {
            conn.Close();
        }
        public void addGatedata(string gatetype,string opentime,string closetime,string opentype)
        {
            try
            {
                SqlCommand com = new SqlCommand("insert into gatelogs (gatetype,opentime,closetime,opentype) values (@gatetype,@opentime,@closetime,@opentype)", conn);
                com.Parameters.AddWithValue("gatetype", gatetype);
                com.Parameters.AddWithValue("opentime", opentime);
                com.Parameters.AddWithValue("closetime", closetime);
                com.Parameters.AddWithValue("opentype", opentype);
                com.ExecuteNonQuery();
            }catch
            {

            }
        }


        public void addSlotusage(int slotno , string starttime , string endtime)
        {
            try
            {
                SqlCommand com = new SqlCommand("insert into slotusage (slotno,starttime,endtime) values (@slotno,@starttime,@endtime)", conn);
            com.Parameters.AddWithValue("slotno", slotno);
            com.Parameters.AddWithValue("starttime", starttime);
            com.Parameters.AddWithValue("endtime", endtime);
            com.ExecuteNonQuery();
            }
            catch
            {

            }
        }

        public void addMaintenanceslotusage(int slotno, string starttime, string endtime)
        {
            try
            {
                SqlCommand com = new SqlCommand("insert into maintenancelog (slotno,starttime,endtime) values (@slotno,@starttime,@endtime)", conn);
            com.Parameters.AddWithValue("slotno", slotno);
            com.Parameters.AddWithValue("starttime", starttime);
            com.Parameters.AddWithValue("endtime", endtime);
            com.ExecuteNonQuery();
            }
            catch
            {

            }
        }

        public SqlDataReader getAllMaintenanceLog()
        {
            SqlDataReader dr = null;
            try
            {
                SqlCommand com = new SqlCommand("select * from maintenancelog", conn);
            dr = com.ExecuteReader();
            return dr;
            }
            catch
            {

            }
            return dr;
        }

        public SqlDataReader getAllMaintenanceLogbySlotNo(int slot)
        {
            SqlDataReader dr = null;
            try
            {
                SqlCommand com = new SqlCommand("select * from maintenancelog where slotno = @slotno", conn);
            com.Parameters.AddWithValue("slotno", slot);
            dr = com.ExecuteReader();
            
            }
            catch
            {

            }
            return dr;
        }

        public SqlDataReader getAllCarUsageLog()
        {
            SqlDataReader dr = null;
            try
            {
                SqlCommand com = new SqlCommand("select * from slotusage", conn);
             dr = com.ExecuteReader();
            
            }
            catch
            {

            }
            return dr;
        }

        public SqlDataReader getAllCarUsageLogbySlotNo(int slot)
        {
            SqlDataReader dr = null;
            try
            {
                SqlCommand com = new SqlCommand("select * from slotusage where slotno = @slotno", conn);
            com.Parameters.AddWithValue("slotno", slot);
             dr = com.ExecuteReader();
           
            }
            catch
            {

            }
            return dr;
        }

        public SqlDataReader getAllGateUsageLog()
        {
            SqlDataReader dr = null;
            try
            {
                SqlCommand com = new SqlCommand("select * from gatelogs", conn);
            dr = com.ExecuteReader();
            
            }
            catch
            {

            }
            return dr;
        }

        public SqlDataReader getGateUsagebytype(string gatetype)
        {
            SqlDataReader dr = null;
            try
            {
                SqlCommand com = new SqlCommand("select * from gatelogs where gatetype = @gatetype", conn);
            com.Parameters.AddWithValue("gatetype", gatetype);
            dr = com.ExecuteReader();
            
            }
            catch
            {

            }
            return dr;
        }

        public SqlDataReader getGateUsagebyopentype(string gateopentype)
        {
            SqlDataReader dr = null;
            try
            {
                SqlCommand com = new SqlCommand("select * from gatelogs where opentype = @opentype", conn);
                com.Parameters.AddWithValue("opentype", gateopentype);
                dr = com.ExecuteReader();
                
            }catch
            {

            }
            return dr;
        }

    }
}
