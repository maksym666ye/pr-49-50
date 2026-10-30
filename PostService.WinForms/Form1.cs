using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using PostService.Dtos;

namespace PostService.WinForms;

public partial class Form1 : Form
{
    private readonly HttpClient _httpClient = new HttpClient
    {
        BaseAddress = new Uri("http://localhost:5201/") 
    };

    private DataGridView _dataGridView = null!;
    private Button _btnRefresh = null!;
    private Button _btnCreate = null!;
    private Button _btnEdit = null!;
    private Button _btnDelete = null!;

    public Form1()
    {
        InitializeComponentCustom();
    }

    private void InitializeComponentCustom()
    {
        this.Text = "PostService - Облік поштових відправлень";
        this.Size = new Size(1050, 580);
        this.StartPosition = FormStartPosition.CenterScreen;

        _btnRefresh = new Button
        {
            Text = "Оновити",
            Location = new Point(12, 12),
            Size = new Size(130, 35),
            Font = new Font("Segoe UI", 9.75F, FontStyle.Bold),
            ForeColor = Color.Black
        };

        _btnCreate = new Button
        {
            Text = "Створити",
            Location = new Point(150, 12),
            Size = new Size(130, 35),
            Font = new Font("Segoe UI", 9.75F, FontStyle.Bold),
            ForeColor = Color.Black
        };

        _btnEdit = new Button
        {
            Text = "Редагувати",
            Location = new Point(288, 12),
            Size = new Size(130, 35),
            Font = new Font("Segoe UI", 9.75F, FontStyle.Bold),
            ForeColor = Color.Black
        };

        _btnDelete = new Button
        {
            Text = "Видалити",
            Location = new Point(426, 12),
            Size = new Size(130, 35),
            Font = new Font("Segoe UI", 9.75F, FontStyle.Bold),
            ForeColor = Color.Black
        };

        _dataGridView = new DataGridView
        {
            Location = new Point(12, 60),
            Size = new Size(1010, 460),
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            ReadOnly = true,
            AllowUserToAddRows = false,
            BackgroundColor = Color.White,
            EnableHeadersVisualStyles = false
        };

        _dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
        _dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
        _dataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

        _dataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 230, 242);
        _dataGridView.DefaultCellStyle.SelectionForeColor = Color.Black;

        _btnRefresh.Click += async (s, e) => await LoadPostingsAsync();
        _btnCreate.Click += async (s, e) => await CreatePostingAsync();
        _btnEdit.Click += async (s, e) => await EditPostingAsync();
        _btnDelete.Click += async (s, e) => await DeleteSelectedPostingAsync();

        this.Controls.Add(_btnRefresh);
        this.Controls.Add(_btnCreate);
        this.Controls.Add(_btnEdit);
        this.Controls.Add(_btnDelete);
        this.Controls.Add(_dataGridView);

        this.Load += async (s, e) => await LoadPostingsAsync();
    }

    private async Task LoadPostingsAsync()
    {
        try
        {
            var postings = await _httpClient.GetFromJsonAsync<List<PostingGetDto>>("postings");
            _dataGridView.DataSource = postings;
            TranslateHeaders();
            _dataGridView.ClearSelection();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Не вдалося завантажити дані:\n{ex.Message}\nПереконайся, що сервер запущеній!", "Помилка мережі", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task CreatePostingAsync()
    {
        using var dialog = new PostingEditForm();
        if (dialog.ShowDialog(this) == DialogResult.OK && dialog.CreatedDto != null)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("postings", dialog.CreatedDto);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Нове відправлення успішно створено!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadPostingsAsync();
                }
                else
                {
                    MessageBox.Show($"Помилка створення. Код: {response.StatusCode}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка запиту: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private async Task EditPostingAsync()
    {
        if (_dataGridView.SelectedRows.Count == 0)
        {
            MessageBox.Show("Оберіть рядок у таблиці для редагування!", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var selectedPosting = (PostingGetDto)_dataGridView.SelectedRows[0].DataBoundItem;
        using var dialog = new PostingEditForm(selectedPosting);

        if (dialog.ShowDialog(this) == DialogResult.OK && dialog.UpdatedDto != null)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"postings/{selectedPosting.Id}", dialog.UpdatedDto);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Зміни успішно збережено!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadPostingsAsync();
                }
                else
                {
                    MessageBox.Show($"Помилка оновлення. Код: {response.StatusCode}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка запиту: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private async Task DeleteSelectedPostingAsync()
    {
        if (_dataGridView.SelectedRows.Count == 0)
        {
            MessageBox.Show("Оберіть рядок у таблиці для видалення!", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var selectedPosting = (PostingGetDto)_dataGridView.SelectedRows[0].DataBoundItem;
        var result = MessageBox.Show($"Ви дійсно бажаєте видалити відправлення #{selectedPosting.Id} (Від: {selectedPosting.From} -> До: {selectedPosting.To})?", 
                                    "Підтвердження видалення", 
                                    MessageBoxButtons.YesNo, 
                                    MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"postings/{selectedPosting.Id}");
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Запис успішно видалено!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadPostingsAsync();
                }
                else
                {
                    MessageBox.Show($"Помилка видалення. Код: {response.StatusCode}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка запиту: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void TranslateHeaders()
    {
        if (_dataGridView.Columns["Id"] != null) _dataGridView.Columns["Id"].HeaderText = "ID";
        if (_dataGridView.Columns["From"] != null) _dataGridView.Columns["From"].HeaderText = "Звідки";
        if (_dataGridView.Columns["To"] != null) _dataGridView.Columns["To"].HeaderText = "Куди";
        if (_dataGridView.Columns["Content"] != null) _dataGridView.Columns["Content"].HeaderText = "Вміст";
        if (_dataGridView.Columns["DeliveryType"] != null) _dataGridView.Columns["DeliveryType"].HeaderText = "Тип доставки";
        if (_dataGridView.Columns["Weight"] != null) _dataGridView.Columns["Weight"].HeaderText = "Вага (кг)";
        if (_dataGridView.Columns["Width"] != null) _dataGridView.Columns["Width"].HeaderText = "Ширина (см)";
        if (_dataGridView.Columns["Height"] != null) _dataGridView.Columns["Height"].HeaderText = "Висота (см)";
        if (_dataGridView.Columns["Depth"] != null) _dataGridView.Columns["Depth"].HeaderText = "Глибина (см)";
        if (_dataGridView.Columns["Value"] != null) _dataGridView.Columns["Value"].HeaderText = "Оцінка (грн)";
        if (_dataGridView.Columns["Price"] != null) _dataGridView.Columns["Price"].HeaderText = "Вартість (грн)";
        if (_dataGridView.Columns["CreatedAt"] != null) _dataGridView.Columns["CreatedAt"].HeaderText = "Дата створення";
    }
}