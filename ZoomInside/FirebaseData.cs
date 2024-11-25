using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoomInside
{
    public class FirebaseData
    {
		private string dangerScale;
		private string info;
		private string label;

		public string Label
		{
			get { return label; }
			set { label = value; }
		}


		public string Info
		{
			get { return info; }
			set { info = value; }
		}


		public string DangerScale
		{
			get { return dangerScale; }
			set { dangerScale = value; }
		}


        public FirebaseData(string dangerScale, string info, string label)
        {
            this.DangerScale = dangerScale;
			this.Info = info;
			this.Label = label;
        }
    }
}
