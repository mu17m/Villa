using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Web.ViewModels;

namespace WhiteLagoon.Application.Common.SD
{
    public static class SD
    {
        public const string Role_Admin = "Admin";
        public const string Role_Customer = "Customer";

        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusCheckedIn = "CheckedIn";
        public const string StatusCompleted = "Completed";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefunded = "Refunded";

        public static int VillaRoomsAvailableCount(int VillaId, List<VillaNumber> villaNumbers, List<Booking> bookings, int nights, DateOnly checkInDate)
        {
            var roomsInVilla = villaNumbers.Where(vn => vn.VillaId == VillaId).Count();
            List<int> roomsBooked = new();
            int finalAvilableRoomsForAllNights = int.MaxValue;

            for(int i=0; i<nights; i++)
            {
                // returns all the bookings for the specific villa for the specific date if the NEW checkInDate is between the checkInDate and checkOutDate of the booked villa
                var VillasBooked = bookings.Where(b => b.CheckInDate <= checkInDate.AddDays(i) 
                && b.CheckOutDate > checkInDate.AddDays(i) && b.VillaId == VillaId);
                foreach(var booking in VillasBooked)
                {
                    if(!roomsBooked.Contains(booking.Id))
                    {
                        roomsBooked.Add(booking.Id);
                    }
                }
                var totalAvilableRooms = roomsInVilla - roomsBooked.Count;

                if(totalAvilableRooms == 0)
                {
                    return 0;
                }
                else
                {
                    // return the minimum number of rooms available for all the nights
                    if (finalAvilableRoomsForAllNights > totalAvilableRooms)
                    {
                        finalAvilableRoomsForAllNights = totalAvilableRooms;
                    }
                }
            }
                return finalAvilableRoomsForAllNights;
        }
        public static RadialChartDto GetRadialChartDataModel(int totalCounts, int countByCurrentMonth, int countByPreviusMonth)
        {
            RadialChartDto radialBarChar = new();
            int IncreaseDecreaseRation = 100;
            if (countByPreviusMonth != 0)
            {
                IncreaseDecreaseRation = Convert.ToInt32(countByCurrentMonth - countByPreviusMonth / countByPreviusMonth * 100);
            }
            radialBarChar.TotalCount = totalCounts;
            radialBarChar.CountInCurrentMonth = countByCurrentMonth;
            radialBarChar.HasRationIncreased = countByCurrentMonth > countByPreviusMonth;
            radialBarChar.Series = new int[] { IncreaseDecreaseRation };

            return radialBarChar;
        }
    }
}
