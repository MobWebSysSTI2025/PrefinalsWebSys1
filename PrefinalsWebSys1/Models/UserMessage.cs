﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace PrefinalsWebSys1.Models;

public partial class UserMessage
{
    public int ID { get; set; }

    public int FromUserID { get; set; }

    public int ToUserID { get; set; }

    public string MessageType { get; set; }

    public string MessageBody { get; set; }

    public int Priorty { get; set; }

    public DateTime SentDate { get; set; }

    public DateTime ReceivedDate { get; set; }

    public bool IsDeleted { get; set; }

    public string CreatedBy { get; set; }

    public string ModifiedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }
}