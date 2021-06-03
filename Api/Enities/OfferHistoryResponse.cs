using Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Enities
{
    public class OfferHistoryResponse
    {
        public OfferHistoryResponse(OfferHistory offerHistory, int type)
        {
            this.JobId = offerHistory.JobId;
            this.FreelancerId = offerHistory.FreelancerId;
            this.OfferPrice = offerHistory.OfferPrice;
            this.ExpectedDay = offerHistory.ExpectedDay;
            this.Description = offerHistory.Description;
            this.TodoList = offerHistory.TodoList;
            try
            {
                this.Freelancer = type ==2 ?new AccountForListResponse(offerHistory.Freelancer):null;
            }
            catch (Exception) { }
            try
            {
                this.Job = type ==1? new JobForListResponse(offerHistory.Job):null;
            }
            catch (Exception) { }
        }
        public int JobId { get; set; }
        public int FreelancerId { get; set; }
        public int OfferPrice { get; set; }
        public string ExpectedDay { get; set; }
        public string Description { get; set; }
        public string TodoList { get; set; }
        public string Status { get; set; }

        public AccountForListResponse Freelancer { get; set; }
        public JobForListResponse Job { get; set; }
    }
}
