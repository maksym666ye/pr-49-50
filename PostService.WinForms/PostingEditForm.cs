using System;
using System.Drawing;
using System.Windows.Forms;
using PostService.CommonTypes;
using PostService.Dtos;

namespace PostService.WinForms;

public class PostingEditForm : Form
{
    private TextBox _txtFrom = null!;
    private TextBox _txtTo = null!;
    private TextBox _txtContent = null!;
    private ComboBox _cmbDeliveryType = null!;
    private TextBox _txtWeight = null!;
    private TextBox _txtWidth = null!;
    private TextBox _txtHeight = null!;
    private TextBox _txtDepth = null!;
    private TextBox _txtValue = null!;

    private Button _btnSave = null!;
    private Button _btnCancel = null!;
    private ErrorProvider _errorProvider = null!;

    public PostingPostDto? CreatedDto { get; private set; }
    public PostingPutDto? UpdatedDto { get; private set; }
    private readonly int? _editingId;

    public PostingEditForm(PostingGetDto? existingPosting = null)
{
    _editingId = existingPosting?.Id;
    InitializeComponentsCustom();

    if (existingPosting != null)
    {
        this.Text = $"Редагування відправлення #{existingPosting.Id}";
        _txtFrom.Text = existingPosting.From;
        _txtTo.Text = existingPosting.To;
        _txtContent.Text = existingPosting.Content;
        _cmbDeliveryType.SelectedItem = existingPosting.DeliveryType;
        _txtWeight.Text = existingPosting.Weight.ToString();
        _txtWidth.Text = existingPosting.Width.ToString();
        _txtHeight.Text = existingPosting.Height.ToString();
        _txtDepth.Text = existingPosting.Depth.ToString();
        _txtValue.Text = existingPosting.Value.ToString();
    }
    else
    {
        this.Text = "Створення нового відправлення";
    }
}

    private void InitializeComponentsCustom()
    {
        this.Size = new Size(400, 500);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;

        _errorProvider = new ErrorProvider { BlinkStyle = ErrorBlinkStyle.NeverBlink };

        var lblFrom = new Label { Text = "Звідки:", Location = new Point(20, 20), AutoSize = true };
        _txtFrom = new TextBox { Location = new Point(140, 18), Width = 210 };

        var lblTo = new Label { Text = "Куди:", Location = new Point(20, 55), AutoSize = true };
        _txtTo = new TextBox { Location = new Point(140, 53), Width = 210 };

        var lblContent = new Label { Text = "Вміст:", Location = new Point(20, 90), AutoSize = true };
        _txtContent = new TextBox { Location = new Point(140, 88), Width = 210 };

        var lblDelivery = new Label { Text = "Тип доставки:", Location = new Point(20, 125), AutoSize = true };
        _cmbDeliveryType = new ComboBox { Location = new Point(140, 123), Width = 210, DropDownStyle = ComboBoxStyle.DropDownList };
        _cmbDeliveryType.DataSource = Enum.GetValues(typeof(DeliveryType));

        var lblWeight = new Label { Text = "Вага (кг):", Location = new Point(20, 160), AutoSize = true };
        _txtWeight = new TextBox { Location = new Point(140, 158), Width = 210, Text = "1" };

        var lblWidth = new Label { Text = "Ширина (см):", Location = new Point(20, 195), AutoSize = true };
        _txtWidth = new TextBox { Location = new Point(140, 193), Width = 210, Text = "10" };

        var lblHeight = new Label { Text = "Висота (см):", Location = new Point(20, 230), AutoSize = true };
        _txtHeight = new TextBox { Location = new Point(140, 228), Width = 210, Text = "10" };

        var lblDepth = new Label { Text = "Глибина (см):", Location = new Point(20, 265), AutoSize = true };
        _txtDepth = new TextBox { Location = new Point(140, 263), Width = 210, Text = "10" };

        var lblValue = new Label { Text = "Цінність (грн):", Location = new Point(20, 300), AutoSize = true };
        _txtValue = new TextBox { Location = new Point(140, 298), Width = 210, Text = "100" };

        _btnSave = new Button
        {
            Text = "Зберегти",
            Location = new Point(140, 370),
            Width = 100,
            Height = 35,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = Color.Black
        };

        _btnCancel = new Button
        {
            Text = "Скасувати",
            Location = new Point(250, 370),
            Width = 100,
            Height = 35,
            DialogResult = DialogResult.Cancel,
            ForeColor = Color.Black
        };

        _btnSave.Click += OnSaveClick;

        this.Controls.AddRange(new Control[] {
            lblFrom, _txtFrom,
            lblTo, _txtTo,
            lblContent, _txtContent,
            lblDelivery, _cmbDeliveryType,
            lblWeight, _txtWeight,
            lblWidth, _txtWidth,
            lblHeight, _txtHeight,
            lblDepth, _txtDepth,
            lblValue, _txtValue,
            _btnSave, _btnCancel
        });
    }

    private void OnSaveClick(object? sender, EventArgs e)
    {
        _errorProvider.Clear();
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(_txtFrom.Text))
        {
            _errorProvider.SetError(_txtFrom, "Заповніть поле 'Звідки'");
            isValid = false;
        }

        if (string.IsNullOrWhiteSpace(_txtTo.Text))
        {
            _errorProvider.SetError(_txtTo, "Заповніть поле 'Куди'");
            isValid = false;
        }

        if (string.IsNullOrWhiteSpace(_txtContent.Text))
        {
            _errorProvider.SetError(_txtContent, "Заповніть поле 'Вміст'");
            isValid = false;
        }

        if (!float.TryParse(_txtWeight.Text, out float weight) || weight <= 0)
        {
            _errorProvider.SetError(_txtWeight, "Введіть число більше 0");
            isValid = false;
        }

        if (!float.TryParse(_txtWidth.Text, out float width) || width <= 0)
        {
            _errorProvider.SetError(_txtWidth, "Введіть число більше 0");
            isValid = false;
        }

        if (!float.TryParse(_txtHeight.Text, out float height) || height <= 0)
        {
            _errorProvider.SetError(_txtHeight, "Введіть число більше 0");
            isValid = false;
        }

        if (!float.TryParse(_txtDepth.Text, out float depth) || depth <= 0)
        {
            _errorProvider.SetError(_txtDepth, "Введіть число більше 0");
            isValid = false;
        }

        if (!float.TryParse(_txtValue.Text, out float val) || val < 0)
        {
            _errorProvider.SetError(_txtValue, "Введіть число від 0");
            isValid = false;
        }

        if (!isValid) return;

        var deliveryType = (DeliveryType)_cmbDeliveryType.SelectedItem!;

        if (_editingId.HasValue)
        {
            UpdatedDto = new PostingPutDto
            {
                Id = _editingId.Value,
                From = _txtFrom.Text,
                To = _txtTo.Text,
                Content = _txtContent.Text,
                DeliveryType = deliveryType,
                Weight = weight,
                Width = width,
                Height = height,
                Depth = depth,
                Value = val
            };
        }
        else
        {
            CreatedDto = new PostingPostDto
            {
                From = _txtFrom.Text,
                To = _txtTo.Text,
                Content = _txtContent.Text,
                DeliveryType = deliveryType,
                Weight = weight,
                Width = width,
                Height = height,
                Depth = depth,
                Value = val
            };
        }

        this.DialogResult = DialogResult.OK;
        this.Close();
    }
}