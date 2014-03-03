using System.Text;

namespace GmailReceiver
{
    public class Address
    {
        public int HouseNumber { get; set; }
        public string StreetDirection { get; set; }
        public string StreetName { get; set; }
        public string StreetType { get; set; }

        public override string ToString()
        {
            StringBuilder addressString = new StringBuilder();
            foreach (var property in typeof(Address).GetProperties())
            {
                var propVal = property.GetValue(this);
                if (propVal != null)
                {
                    addressString.Append(propVal + " ");
                }
            }
            return addressString.ToString();
        }
    }
}