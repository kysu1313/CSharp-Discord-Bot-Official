using System;

namespace ClassLibrary.Models.ContextModels
{
    public interface ICryptoModel
    {
        int id { get; set; }
        string coinName { get; set; }
        float price1 { get; set; }
        float price2 { get; set; }
        float price3 { get; set; }
        float price4 { get; set; }
        float price5 { get; set; }
        DateTime dateUpdated { get; set; }
    }
}