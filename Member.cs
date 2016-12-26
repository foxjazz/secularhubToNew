using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertDataToNewFormat
{
    public class IPayment
    {
        public DateTime receivedDate;
        public double amount;
        public string type;
        public bool active;
        public DateTime targetDate;
    }
    public class ExtendedMember
    {
        public string firstName;
        public string lastName;
        public string email;
        public string address;
        public string city;
        public string state;
        public string zip;
        public string notes;
    }
    public class Member
    {
        /*
         * index: number;
    uuid: string;
    firstName: string;
    lastName: string;
    phone: string;
    email: string;
    address: string;
    city: string;
    state: string;
    zip: string;
    createdAt: number;
    joinedDate: Date;
    completed: boolean;
    active: boolean;
    frequency: number;
    durationmonths: number;
    targetDate: Date;
    Description: string;
    Notes: string;
    ExtendedMembers: Array<ExtendedMember>;
    payments: Array<IPayment>;
         * */

        public Int32 index;
        public string memberID;
        public string contactID;
        public string personID;
        public string firstName;
        public string lastName;
        public string phone;
        public string email;
        public string address;
        public string city;
        public string state;
        public string zip;
        public double createdAt;
        public DateTime joinedDate;
        public bool completed;
        public bool active;
        public Int32 frequency;
        public Int32 durationmonths;
        public DateTime targetDate;
        public string Description;
        public string Notes;
        public List<ExtendedMember> extendedMembers;
        public List<IPayment> payments;



    }
}
