using SimpleAuthSystem.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace SimpleAuthSystem.Model{
    public class BruteForceResponse{
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public List<data> Data { get; set; }
    }
}