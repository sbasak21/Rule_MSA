using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RulesMicroservice.Controllers;
using RulesMicroservice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RulesMicroservice.Services
{
    public class MonthlyJobService : IMonthlyJobService
    {
        // static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(MonthlyjobService));
        private readonly IConfiguration _config;

        public MonthlyJobService(IConfiguration config)
        {
            _config = config;
        }
        public async Task toRunMonthlyjob()
        {
            List<Account> allAccounts = getAccounts();
            foreach (var account in allAccounts)
            {
                if (account.Balance < 5000 && account.Balance > 0)
                {
                    RulesController rulesController = new RulesController();
                    float ServiceCharge = rulesController.GetServiceCharges(account.AccountType);
                    TransactionDTO withDraw = new TransactionDTO() { AccId = account.AccId, Amount = (double)ServiceCharge };
                    TransactionStatusForDepositAndWithDraw status = await ApplyServiceCharge(withDraw);
                    if (status.Status == "Withdraw Successfull")
                    {
                        //_log4net.Info("Service charge deducted for the AccountID = " + account.AccountID);
                    }
                    else
                    {
                        // _log4net.Info("Some Issue occured while deducting service charge for the AccountID = " + account.AccountID);
                    }
                }
            }
        }

        /// <summary>
        /// It get all the accounts that the bank has from the account management client
        /// </summary>
        /// <returns>list of accounts</returns>
        public List<Account> getAccounts()
        {
            List<Account> accounts = new List<Account>();
            HttpResponseMessage response = getAccountResponse();

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                accounts = JsonConvert.DeserializeObject<List<Account>>(result);
                //_log4net.Info("Got All Accounts");
            }

            return accounts;
        }


        /// <summary>
        /// It get response from the client to get all the accounts
        /// </summary>
        /// <returns>Response message from the client</returns>
        public HttpResponseMessage getAccountResponse()
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(_config.GetValue<string>("ClientConnection:accountApiConn"));
                HttpResponseMessage response = client.GetAsync("api/AccountManagement/getAllCustomerAccounts").Result;
                //_log4net.Info("Get All accounts response created");
                return response;
            }
            catch (Exception exception)
            {
                //_log4net.Error(exception.Message);
                throw exception;
            }

        }

        /// <summary>
        /// It applies service charge for the account that doesn't maintain the minimum balance
        /// </summary>
        /// <param name="withDraw"></param>
        /// <returns>The transaction status if it is successful</returns>
        public async Task<TransactionStatusForDepositAndWithDraw> ApplyServiceCharge(TransactionDTO withDraw)
        {

            TransactionStatusForDepositAndWithDraw withDrawStatus = new TransactionStatusForDepositAndWithDraw();
            HttpResponseMessage result = ApplyServiceChargeResponse(withDraw);

            if (result.IsSuccessStatusCode)
            {
                var apiResponse = await result.Content.ReadAsStringAsync();
                withDrawStatus = JsonConvert.DeserializeObject<TransactionStatusForDepositAndWithDraw>(apiResponse);
                //  _log4net.Info("Service Charge detected Successfully");

            }

            return withDrawStatus;
        }

        /// <summary>
        /// It get response from the client to withdraw the service charges
        /// </summary>
        /// <param name="withDraw"></param>
        /// <returns>Response message from the client</returns>

        public HttpResponseMessage ApplyServiceChargeResponse(TransactionDTO withDraw)
        {
            HttpResponseMessage result = new HttpResponseMessage();

            string uriConn = _config.GetValue<string>("ClientConnection:accountApiConn");
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(uriConn + "api/AccountManagement/withdraw/");
                // _log4net.Info(client.BaseAddress);
                // string token = generateJSONWebToken();
                //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                try
                {
                    //   var response = client.PostAsJsonAsync<TransactionDTO>(client.BaseAddress, withDraw);
                    // response.Wait();
                    //result = response.Result;
                }
                catch (Exception e)
                {
                    //_log4net.Error("Exception Occured" + e);
                    return null;
                }
            }

            // _log4net.Info("Apply Service Charge response created");

            return result;
        }


        //public string generateJSONWebToken()
        //{
        //    _log4net.Info("Generating Token Started..");


        //    try
        //    {
        //        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        //        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


        //        var token = new JwtSecurityToken(
        //          issuer: _config["Jwt:Issuer"],
        //          audience: _config["Jwt:Issuer"],
        //          null,
        //          expires: DateTime.Now.AddMinutes(30),
        //          signingCredentials: credentials);

        //        return new JwtSecurityTokenHandler().WriteToken(token);
        //    }
        //    catch (Exception exception)
        //    {
        //        _log4net.Error(exception.Message);
        //        return null;
        //    }

        //}
    }
}