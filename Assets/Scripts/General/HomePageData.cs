// using System.Collections.Generic;
// using UnityEngine;
// using TMPro;
// using UnityEngine.UI;
// using MySql.Data.MySqlClient;
// public class HomePageData : MonoBehaviour
// {
    

//     public TMP_InputField inputField;

//     public void StoreName(){
//         Debug.Log(inputField.text);
//         int Score =0;
//         MySqlConnection con; //Sql Connection
//         MySqlDataReader reader; //Sql reader

//         try
//             {
//                 string conString = "server=remotemysql.com;database=uwtd5SaD6K;userid=uwtd5SaD6K;password=wXMG3Hpq3Q"; //Login
//                 con = new MySqlConnection(conString);
//                 con.Open();
//             }
//             catch (Exception)
//             {

//                 throw;
//             }

//             if (con.State == ConnectionState.Open) //We dont want to run this code if the connection isnt open
//             {
//                 string CommandSend = $"INSERT INTO Images (`Name`,`Score`) VALUES ('{inputField.text}','{Score}')"; //Creating the string that becomes the sqlquery
//                 MySqlCommand command = new MySqlCommand();
//                 command.Connection = con;
//                 command.CommandText = CommandSend;
//                 reader = command.ExecuteReader(); //executing the command
//                 while (reader.Read()){}
//                 reader.Close();//Closing down the reader
//                 con.Close(); //Closing down the connection
//                 con.Dispose(); //Disposing of excess data
//             }
//     }
    
// }
