using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using SimpleAuthSystem.Model;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using Ubiety.Dns.Core;

namespace SimpleAuthSystem.DataAccessLayer{
    public class data{
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public System.Byte IsCorrect { get; set; }
    }

    public class AuthDL : IAuthDL{
        public readonly IConfiguration _configuration;
        public readonly MySqlConnection _mySqlConnection;

        public AuthDL(IConfiguration configuration){
            _configuration = configuration;
        }

        //--------------------------------------------------------------SignIn--------------------------------------------------------------
        public Task<SignInResponse> SignIn(SignInRequest request){
            SignInResponse response = new SignInResponse();
            response.IsSuccess = true;
            response.Message = "Successful";

            try
            {
                using (SqlConnection con = new SqlConnection("Server=PURJAXX;Database=MidtermProject;Trusted_Connection=True;"))
                {
                    int recordsAffected = 0;
                    SqlCommand command = new SqlCommand($"SELECT COUNT(*) FROM [user] as u WHERE u.UserName=@UserName AND u.PassWord=@PassWord", con);

                    try
                    {
                        con.Open();
                        command.Parameters.AddWithValue("@UserName", request.UserName);
                        command.Parameters.AddWithValue("@PassWord", request.Password);
                        recordsAffected = (int)command.ExecuteScalar();
                    }
                    catch
                    {

                    }
                    finally
                    {
                        con.Close();
                    }

                    if (recordsAffected == 0)
                    {                           //----------Successful Login
                        response.Message = "Login Unsuccessfully";
                        response.IsSuccess = false;

                        //Adding Fail Log To The Database  -----FOR BRUTE FORCE TEST-----
                        SqlCommand commandForLog = new SqlCommand($"INSERT INTO logs (UserName, PassWord, IsCorrect) VALUES (@UserName, @PassWord, @IsCorrect)", con);

                        try
                        {
                            con.Open();
                            commandForLog.Parameters.AddWithValue("@UserName", request.UserName);
                            commandForLog.Parameters.AddWithValue("@PassWord", request.Password);
                            commandForLog.Parameters.AddWithValue("@IsCorrect", false);
                            commandForLog.ExecuteScalar();
                        }
                        catch
                        {

                        }
                        finally
                        {
                            con.Close();
                        }
                    }
                    else
                    {                                              //----------Failed Login
                        response.Message = "Login Successfully";

                        //Adding Success Log To The Database  -----FOR BRUTE FORCE TEST-----
                        SqlCommand commandForLog = new SqlCommand($"INSERT INTO logs (UserName, PassWord, IsCorrect) VALUES (@UserName, @PassWord, @IsCorrect)", con);

                        try
                        {
                            con.Open();
                            commandForLog.Parameters.AddWithValue("@UserName", request.UserName);
                            commandForLog.Parameters.AddWithValue("@PassWord", request.Password);
                            commandForLog.Parameters.AddWithValue("@IsCorrect", true);
                            commandForLog.ExecuteScalar();
                        }
                        catch
                        {

                        }
                        finally
                        {
                            con.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            finally
            {
            }

            return Task.FromResult(response);
        }


        //-------------------------------------------------------------Sign Up-------------------------------------------------------------
        public async Task<SignUpResponse> SignUp(SignUpRequest request){
            SignUpResponse response = new SignUpResponse();
            response.IsSuccess = true;
            response.Message = "All Good";

            try
            {
                using (SqlConnection con = new SqlConnection("Server=PURJAXX;Database=MidtermProject;Trusted_Connection=True;"))
                {
                    SqlCommand command = new SqlCommand($"INSERT INTO [user] (UserName, PassWord) VALUES (@UserName, @PassWord)", con);
                    con.Open();
                    command.Parameters.AddWithValue("@UserName", request.UserName);
                    command.Parameters.AddWithValue("@PassWord", request.Password);
                    command.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }



        //---------------------------------------------------Brute Force Get Request Handler---------------------------------------------------
        public async Task<BruteForceResponse> BruteForceGetHandler(){
            BruteForceResponse response = new BruteForceResponse();
            response.IsSuccess = true;
            response.Message = "Successful";

            try{
                using (SqlConnection con = new SqlConnection("Server=PURJAXX;Database=MidtermProject;Trusted_Connection=True;")){
                    SqlCommand command = new SqlCommand($"SELECT * FROM logs", con);

                    try{
                        con.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        List<data> result = new List<data>();

                        while (reader.Read()){
                            var d = new data();
                            d.UserName = (string)reader[0];
                            d.PassWord = (string)reader[1];
                            d.IsCorrect = (System.Byte)reader[2];
                            result.Add(d);
                        }

                        reader.Close();
                        response.Data = result;
                    }catch{

                    }finally{
                        con.Close();
                    }
                }
            }catch (Exception ex){
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            finally
            {
            }

            return response;
        }
    }
}