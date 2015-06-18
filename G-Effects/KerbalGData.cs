using System;

namespace G_Effects
{
	/// <summary>
	/// Holds personal values of G effects per kerbal
	/// </summary>
	public class KerbalGData
	{
		
		/// This is the cumulative effect of G's which is increasingly negative to represent a redout condition 
        /// and increasingly positive to represent a blackout condition. As G forces continue, the cumulativeG
        /// value is increased, by larger amounts for higher current G levels, to allow for longer periods of G
        /// force tolerance at lower G levels.
        public double cumulativeG = 0;
		public int needBreath = 0;
		public int gLocFadeAmount = 0;
						
		public KerbalGData()
		{
		}
		
	}
}
