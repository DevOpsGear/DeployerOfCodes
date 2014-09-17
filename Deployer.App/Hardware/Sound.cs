using System;
using Deployer.Services.Hardware;
using Gadgeteer.Modules.GHIElectronics;

namespace Deployer.App.Hardware
{
	public class Sound : ISound
	{
		private readonly Tunes _tunes;

		private const double C0 = 16.35;
		private const double Csh0_Db0 = 17.32;
		private const double D0 = 18.35;
		private const double Dsh0_Eb0 = 19.45;
		private const double E0 = 20.6;
		private const double F0 = 21.83;
		private const double Fsh0_Gb0 = 23.12;
		private const double G0 = 24.5;
		private const double Gsh0_Ab0 = 25.96;
		private const double A0 = 27.5;
		private const double Ash0_Bb0 = 29.14;
		private const double B0 = 30.87;
		private const double C1 = 32.7;
		private const double Csh1_Db1 = 34.65;
		private const double D1 = 36.71;
		private const double Dsh1_Eb1 = 38.89;
		private const double E1 = 41.2;
		private const double F1 = 43.65;
		private const double Fsh1_Gb1 = 46.25;
		private const double G1 = 49;
		private const double Gsh1_Ab1 = 51.91;
		private const double A1 = 55;
		private const double Ash1_Bb1 = 58.27;
		private const double B1 = 61.74;
		private const double C2 = 65.41;
		private const double Csh2_Db2 = 69.3;
		private const double D2 = 73.42;
		private const double Dsh2_Eb2 = 77.78;
		private const double E2 = 82.41;
		private const double F2 = 87.31;
		private const double Fsh2_Gb2 = 92.5;
		private const double G2 = 98;
		private const double Gsh2_Ab2 = 103.83;
		private const double A2 = 110;
		private const double Ash2_Bb2 = 116.54;
		private const double B2 = 123.47;
		private const double C3 = 130.81;
		private const double Csh3_Db3 = 138.59;
		private const double D3 = 146.83;
		private const double Dsh3_Eb3 = 155.56;
		private const double E3 = 164.81;
		private const double F3 = 174.61;
		private const double Fsh3_Gb3 = 185;
		private const double G3 = 196;
		private const double Gsh3_Ab3 = 207.65;
		private const double A3 = 220;
		private const double Ash3_Bb3 = 233.08;
		private const double B3 = 246.94;
		private const double C4 = 261.63;
		private const double Csh4_Db4 = 277.18;
		private const double D4 = 293.66;
		private const double Dsh4_Eb4 = 311.13;
		private const double E4 = 329.63;
		private const double F4 = 349.23;
		private const double Fsh4_Gb4 = 369.99;
		private const double G4 = 392;
		private const double Gsh4_Ab4 = 415.3;
		private const double A4 = 440;
		private const double Ash4_Bb4 = 466.16;
		private const double B4 = 493.88;
		private const double C5 = 523.25;
		private const double Csh5_Db5 = 554.37;
		private const double D5 = 587.33;
		private const double Dsh5_Eb5 = 622.25;
		private const double E5 = 659.25;
		private const double F5 = 698.46;
		private const double Fsh5_Gb5 = 739.99;
		private const double G5 = 783.99;
		private const double Gsh5_Ab5 = 830.61;
		private const double A5 = 880;
		private const double Ash5_Bb5 = 932.33;
		private const double B5 = 987.77;
		private const double C6 = 1046.5;
		private const double Csh6_Db6 = 1108.73;
		private const double D6 = 1174.66;
		private const double Dsh6_Eb6 = 1244.51;
		private const double E6 = 1318.51;
		private const double F6 = 1396.91;
		private const double Fsh6_Gb6 = 1479.98;
		private const double G6 = 1567.98;
		private const double Gsh6_Ab6 = 1661.22;
		private const double A6 = 1760;
		private const double Ash6_Bb6 = 1864.66;
		private const double B6 = 1975.53;
		private const double C7 = 2093;
		private const double Csh7_Db7 = 2217.46;
		private const double D7 = 2349.32;
		private const double Dsh7_Eb7 = 2489.02;
		private const double E7 = 2637.02;
		private const double F7 = 2793.83;
		private const double Fsh7_Gb7 = 2959.96;
		private const double G7 = 3135.96;
		private const double Gsh7_Ab7 = 3322.44;
		private const double A7 = 3520;
		private const double Ash7_Bb7 = 3729.31;
		private const double B7 = 3951.07;
		private const double C8 = 4186.01;
		private const double Csh8_Db8 = 4434.92;
		private const double D8 = 4698.63;
		private const double Dsh8_Eb8 = 4978.03;
		private const double E8 = 5274.04;
		private const double F8 = 5587.65;
		private const double Fsh8_Gb8 = 5919.91;
		private const double G8 = 6271.93;
		private const double Gsh8_Ab8 = 6644.88;
		private const double A8 = 7040;
		private const double Ash8_Bb8 = 7458.62;
		private const double B8 = 7902.13;

		private const int Whole = 200;
		private const int Half = 100;
		private const int Quarter = 50;

		public Sound(Tunes tunes)
		{
			_tunes = tunes;
		}

		public void SoundAlarm()
		{
			var melody = new Tunes.Melody();

			melody.Add((int) C4, Quarter);
			melody.Add((int) F4, Quarter);
			melody.Add((int) C5, Half);

			_tunes.Play(melody);
		}

		public void SoundSuccess()
		{
			for (var freq = (int) A3; freq < (int) A5; freq++)
			{
				_tunes.Play(freq, 10);
			}
			for (var freq = (int) A5; freq > (int) A3; freq--)
			{
				_tunes.Play(freq, 10);
			}
		}

		public void SoundRandom()
		{
			var rnd = new Random();
			//var melody = new Tunes.Melody();
			for (var idx = 0; idx < 4000; idx++)
			{
				var freq = (rnd.NextDouble()*2000.0) + 60.0;
				//melody.Add(new Tunes.MusicNote(new Tunes.Tone(freq), 10));
				_tunes.Play((int) freq, 10);
			}
			//_tunes.Play(melody);
		}

		public void SoundFailure()
		{
			for (var idx = 0; idx < 100; idx++)
			{
				_tunes.Play(60, 100);
				_tunes.Play(30, 100);
			}
		}
	}
}