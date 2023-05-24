using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
	public partial class MainForm : Form
	{
		private List<Country> countries;
		private List<Hotel> hotels;

		public MainForm()
		{
			InitializeComponent();
		}

		private void MainForm_Load (object sender, EventArgs e)
		{
			// Загрузка данных из файлов
			countries = LoadCountriesFromFile("countries.txt");
			hotels = LoadHotelsFromFile("hotels.txt");

			if ( countries.Count == 0 || hotels.Count == 0 )
			{
				MessageBox.Show("Файлы не содержат данных.");
				return;
			}
		}

		private void displayButton_Click (object sender, EventArgs e)
		{
			DisplayData();
		}

		private void deleteButton_Click (object sender, EventArgs e)
		{
			if (dataGridView1.SelectedRows.Count > 0)
			{
				int recordId = (int) dataGridView1.SelectedRows [ 0 ].Cells [ 0 ].Value;

				countries.RemoveAll(country => country.Id == recordId);
				hotels.RemoveAll(hotel => hotel.Id == recordId);

				int sortParameter = GetSortParameter( );
				var updatedResult = from country in countries
									join hotel in hotels on country.Id equals hotel.Id
									orderby GetOrderBy(sortParameter, country)
									select new
									{
										Pid = country.Id,
										HotelName = hotel.Name,
										Country = country.Name,
										Quantity = hotel.Type.Length,
										Price = hotel.Type.Length * hotel.Price
									};

				dataGridView1.DataSource = null;
				dataGridView1.DataSource = updatedResult.ToList( );
				SetColumnsHeader();

				MessageBox.Show("Запись успешно удалена.", "Внимание!");
			}
			else MessageBox.Show("Выберите запись для удаления.", "Внимание!");
		}

		private void DisplayData()
		{
			int sortParameter = GetSortParameter();

			var result = from country in countries
						 join hotel in hotels on country.Id equals hotel.Id
						 orderby GetOrderBy(sortParameter, country)
						 select new
						 {
							 Pid = country.Id,
							 HotelName = hotel.Name,
							 Country = country.Name,
							 Quantity = hotel.Type.Length,
							 Price = hotel.Type.Length * hotel.Price
						 };

			dataGridView1.DataSource = result.ToList();
			SetColumnsHeader();
		}

		private void SetColumnsHeader()
		{
			dataGridView1.Columns [ 0 ].HeaderText = "Идентификатор путевки";
			dataGridView1.Columns [ 1 ].HeaderText = "Название отеля";
			dataGridView1.Columns [ 2 ].HeaderText = "Страна";
			dataGridView1.Columns [ 3 ].HeaderText = "Количество";
			dataGridView1.Columns [ 4 ].HeaderText = "Цена";
		}

		private List<Country> LoadCountriesFromFile (string filePath)
		{
			List<Country> countries = new List<Country>();

			if (!File.Exists(filePath))
			{
				MessageBox.Show($"Файл {filePath} не существует.");
				return countries;
			}

			try
			{
				string [ ] lines = File.ReadAllLines(filePath);

				foreach ( string line in lines )
				{
					string [ ] parts = line.Split(';');
					int id = int.Parse(parts [ 0 ]);
					string name = parts [ 1 ];
					countries.Add(new Country { Id = id, Name = name });
				}
				return countries;
			} catch(Exception ex) { MessageBox.Show(ex.ToString()); Application.Exit( ); return null; }
			
		}

		private List<Hotel> LoadHotelsFromFile (string filePath)
		{
			List<Hotel> hotels = new List<Hotel>();

			if (!File.Exists(filePath))
			{
				MessageBox.Show($"Файл {filePath} не существует.");
				return hotels;
			}

			string [] lines = File.ReadAllLines(filePath);

			try
			{
				foreach ( string line in lines )
				{
					string [ ] parts = line.Split(';');
					string type = parts [ 0 ];
					int id = int.Parse(parts [ 1 ]);
					string name = parts [ 2 ];
					int price = int.Parse(parts [ 3 ]);
					hotels.Add(new Hotel { Type = type, Id = id, Name = name, Price = price });
				}
				return hotels;
			} catch ( Exception ex ) { MessageBox.Show(ex.ToString( )); Application.Exit( ); return null; }
		}

		private int GetSortParameter()
		{
			int sortParameter = 1; // По умолчанию сортировка по идентификатору

			if (checkBox1.Checked)
			{
				sortParameter = 2; // Сортировка по названию страны
			}

			return sortParameter;
		}

		private object GetOrderBy(int sortParameter, Country country)
		{
			if (sortParameter == 1) { return country.Id; }
			else { return country.Name; }
		}

	}
}
