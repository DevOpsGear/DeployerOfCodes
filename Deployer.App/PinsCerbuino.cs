using Microsoft.SPOT.Hardware;

namespace Deployer.App
{
	// https://www.ghielectronics.com/community/forum/topic?id=9534
	internal class PinsCerbuino
	{
		// Conflicts with Ethernet // public const Cpu.Pin D0 = (Cpu.Pin) 0x1B; // PB11/I2C2_SDA/UART3_RX
		// Conflicts with Ethernet // public const Cpu.Pin D1 = (Cpu.Pin) 0x1A; // PB10/I2C2_SCL/SPI2_SCK/UART3_TX
		public const Cpu.Pin D2 = (Cpu.Pin) 0x1C; // PB12/CAN2_RX
		// Conflicts with something // public const Cpu.Pin D3 = (Cpu.Pin) 0x2E; // PC14/XO32
		public const Cpu.Pin D4 = (Cpu.Pin) 0x2F; // PC15/XI32
		public const Cpu.Pin D5 = (Cpu.Pin) 0x08; // PA8/MC01/PWM
		public const Cpu.Pin D6 = (Cpu.Pin) 0x0A; // PA10/LOADER1U2/PWM
		public const Cpu.Pin D7 = (Cpu.Pin) 0x24; // PC4/ADC12_IN14
		public const Cpu.Pin D8 = (Cpu.Pin) 0x1D; // PB13/SP2_SCK/CAN2_TX/PWM
		public const Cpu.Pin D9 = (Cpu.Pin) 0x09; // PA9/VBUS
		public const Cpu.Pin D10 = (Cpu.Pin) 0x0F; // PA15/J_TDI/PWM/SP1_SSEL
		// Conflicts with Ethernet // public const Cpu.Pin D11 = (Cpu.Pin) 0x15; // PB5/CAN2_RX/PWM/SP1_MOSI
		// Conflicts with Ethernet // public const Cpu.Pin D12 = (Cpu.Pin) 0x14; // PB4/J_TRST/PWM/SP1_MISO
		// Conflicts with Ethernet // public const Cpu.Pin D13 = (Cpu.Pin) 0x13; // PB3/J_TDO/PWM/SPI1_SCK

		public const Cpu.Pin A0 = (Cpu.Pin) 0x11; // PB1/ADC12_IN9
		public const Cpu.Pin A1 = (Cpu.Pin) 0x05; // PA5/SPI1_SCK/ADC12_IN5/DAC2
		public const Cpu.Pin A2 = (Cpu.Pin) 0x10; // PB0/ADC12_IN8
		public const Cpu.Pin A3 = (Cpu.Pin) 0x23; // PC3/ADC123_IN13/SPI2_MOSI
		public const Cpu.Pin A4 = (Cpu.Pin) 0x21; // PC1/ADC123_IN11
		public const Cpu.Pin A5 = (Cpu.Pin) 0x04; // PA4/SP1_SSEL/ADC12_IN14/DAC1
	}
}