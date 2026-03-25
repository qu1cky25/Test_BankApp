let accounts = [];
let current_account_id = null;

function generate_account_number() {
    let num = "";
    for (let i = 0; i < 20; i++) num += Math.floor(Math.random() * 10);
    return num;
}

function showScreen(id) {
    document.querySelectorAll('.screen').forEach(s => s.classList.add('hidden'));
    const target_screen = document.getElementById(id);
    if (target_screen) target_screen.classList.remove('hidden');

    if (id === 'screen-home') fetch_and_render_home();
}

async function fetch_and_render_home() {
    try {
        const response = await fetch('/api/accounts');
        accounts = await response.json();
        render_home();
    } catch (error) {
        alert("Ошибка подключения к серверу");
    }
}

function render_home() {
    const container = document.getElementById('accounts-container');
    const total_el = document.getElementById('total-amount');
    let total = 0;
    container.innerHTML = "";

    accounts.forEach(acc => {
        total += acc.balance;
        const div = document.createElement('div');
        div.className = 'account-tile';
        div.onclick = () => openDetails(acc.id);
        div.innerHTML = `
        <div>
        <strong>${acc.bank}</strong><br>
        <small style="color:#888">•• ${acc.id.slice(-4)}</small>
        </div>
        <div style="font-weight:600">${acc.balance.toLocaleString()} ₽</div>
        `;
        container.appendChild(div);
    });
    total_el.innerText = `${total.toLocaleString()} ₽`;
}

function openDetails(id) {
    current_account_id = id;
    const acc = accounts.find(a => a.id === id);
    const view = document.getElementById('selected-account-view');
    view.innerHTML = `
    <h2 style="margin-bottom:4px">${acc.bank}</h2>
    <p style="color:#888; margin-bottom:24px">${acc.id}</p>
    <h1 style="font-size:42px; margin-bottom:40px">${acc.balance.toLocaleString()} ₽</h1>
    <div style="display:flex; gap: 10px;">
    <button class="btn-action" onclick="prepareTransfer()">Перевести</button>
    <button class="btn-action" onclick="prepareTopup()">Пополнить</button>
    </div>
    `;
    showScreen('screen-details');
}

function prepareTransfer() {
    document.getElementById('transfer-amount').value = '';
    showScreen('screen-transfer');
}

function prepareTopup() {
    const select = document.getElementById('topup-source');
    const amount_input = document.getElementById('topup-amount');
    amount_input.value = '';
    select.innerHTML = '<option value="">Выберите источник</option>';

    const other_accounts = accounts.filter(a => a.id !== current_account_id);
    if (other_accounts.length === 0) {
        alert("У вас нет других счетов для списания.");
        return;
    }

    other_accounts.forEach(a => {
        const opt = document.createElement('option');
        opt.value = a.id;
        opt.innerText = `${a.bank} (••${a.id.slice(-4)}) — ${a.balance.toLocaleString()} ₽`;
        select.appendChild(opt);
    });
    showScreen('screen-topup');
}

async function addNewAccount() {
    const bank = document.getElementById('new-account-bank').value;
    const new_id = generate_account_number();

    const response = await fetch('/api/accounts', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ id: new_id, bank: bank })
    });

    if (response.ok) {
        showScreen('screen-home');
    } else {
        const err = await response.text();
        alert(err);
    }
}

async function processTransaction(type) {
    const amount_input = document.getElementById(`${type}-amount`);
    const amount = parseFloat(amount_input.value);

    if (isNaN(amount) || amount <= 0) {
        return alert("Введите положительную сумму!");
    }

    let payload = { amount: amount };

    if (type === 'transfer') {
        const target_number = document.querySelector('#screen-transfer input[type="text"]').value;
        if (!target_number || target_number.length !== 20) return alert("Введите корректный номер (20 цифр)");
        payload.from_account_id = current_account_id;
        payload.to_account_id = target_number;
    } else if (type === 'topup') {
        const source_id = document.getElementById('topup-source').value;
        if (!source_id) return alert("Выберите счет списания!");
        payload.from_account_id = source_id;
        payload.to_account_id = current_account_id;
    }

    const response = await fetch(`/api/transactions/${type}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(payload)
    });

    if (response.ok) {
        showScreen('screen-home');
    } else {
        const error_text = await response.text();
        alert(error_text.replace(/"/g, ''));
    }
}

fetch_and_render_home();