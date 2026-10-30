const API_URL = 'http://localhost:5201/postings';

async function loadPostings() {
    try {
        const res = await fetch(API_URL);
        const postings = await res.json();
        const tbody = document.querySelector('#postingsTable tbody');
        tbody.innerHTML = '';

        postings.forEach(p => {
            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td>${p.id}</td>
                <td>${p.from}</td>
                <td>${p.to}</td>
                <td>${p.content}</td>
                <td>${p.deliveryType === 1 ? "Кур'єр" : "Поштомат"}</td>
                <td>${p.weight}</td>
                <td>${p.price}</td>
                <td>
                    <button class="btn primary" onclick="location.href='edit.html?id=${p.id}'">✏️</button>
                    <button class="btn danger" onclick="deletePosting(${p.id})">❌</button>
                </td>
            `;
            tbody.appendChild(tr);
        });
    } catch (err) {
        alert('Помилка зєднання з сервером PostService!');
    }
}

async function checkEditMode() {
    const urlParams = new URLSearchParams(window.location.search);
    const id = urlParams.get('id');
    if (id) {
        document.getElementById('formTitle').innerText = `Редагування відправлення #${id}`;
        document.getElementById('postingId').value = id;
        try {
            const res = await fetch(`${API_URL}/${id}`);
            if (res.ok) {
                const p = await res.json();
                document.getElementById('from').value = p.from;
                document.getElementById('to').value = p.to;
                document.getElementById('content').value = p.content;
                document.getElementById('deliveryType').value = p.deliveryType;
                document.getElementById('weight').value = p.weight;
                document.getElementById('width').value = p.width;
                document.getElementById('height').value = p.height;
                document.getElementById('depth').value = p.depth;
                document.getElementById('value').value = p.value;
            }
        } catch (e) {
            console.error(e);
        }
    }
}

async function savePosting(event) {
    event.preventDefault();
    clearErrors();

    const id = document.getElementById('postingId').value;
    const from = document.getElementById('from').value.trim();
    const to = document.getElementById('to').value.trim();
    const content = document.getElementById('content').value.trim();
    const weight = parseFloat(document.getElementById('weight').value);

    let isValid = true;

    if (!from) {
        showError('from', 'errorFrom', '⚠️ Заповніть поле Звідки');
        isValid = false;
    }
    if (!to) {
        showError('to', 'errorTo', '⚠️ Заповніть поле Куди');
        isValid = false;
    }
    if (!content) {
        showError('content', 'errorContent', '⚠️ Заповніть поле Вміст');
        isValid = false;
    }
    if (isNaN(weight) || weight <= 0) {
        showError('weight', 'errorWeight', '⚠️ Вага повинна бути більше 0');
        isValid = false;
    }

    if (!isValid) return;

    const data = {
        id: id ? parseInt(id) : 0,
        from,
        to,
        content,
        deliveryType: parseInt(document.getElementById('deliveryType').value),
        weight: weight,
        width: parseFloat(document.getElementById('width').value),
        height: parseFloat(document.getElementById('height').value),
        depth: parseFloat(document.getElementById('depth').value),
        value: parseFloat(document.getElementById('value').value)
    };

    const method = id ? 'PUT' : 'POST';
    const url = id ? `${API_URL}/${id}` : API_URL;

    try {
        const res = await fetch(url, {
            method: method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });

        if (res.ok) {
            alert(id ? 'Зміни збережено!' : 'Відправлення створено!');
            window.location.href = 'postings.html';
        } else {
            alert('Помилка збереження на сервері!');
        }
    } catch (e) {
        alert('Помилка запиту!');
    }
}

async function deletePosting(id) {
    if (confirm(`Ви дійсно бажаєте видалити відправлення #${id}?`)) {
        try {
            const res = await fetch(`${API_URL}/${id}`, { method: 'DELETE' });
            if (res.ok) {
                loadPostings();
            }
        } catch (e) {
            alert('Помилка видалення!');
        }
    }
}

function showError(inputId, errorId, msg) {
    document.getElementById(inputId).classList.add('invalid');
    document.getElementById(errorId).innerText = msg;
}

function clearErrors() {
    document.querySelectorAll('.error-msg').forEach(e => e.innerText = '');
    document.querySelectorAll('input').forEach(i => i.classList.remove('invalid'));
}