using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantPOS.Shared.DTOs.Table;

public class TableDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string Status { get; set; } = string.Empty;   // enum-কে string হিসেবে পাঠাবো, ক্লায়েন্ট-ফ্রেন্ডলি
    public int BranchId { get; set; }
}