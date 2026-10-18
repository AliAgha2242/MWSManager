function RequetsMessageCreator() {
    const container = document.getElementById('parametersContainer');
    function addRow() {

        const rows = container.querySelectorAll('.form-row');
        const lastRow = rows[rows.length - 1];

        const name = lastRow.querySelector('.param-name');
        const value = lastRow.querySelector('.param-value');

        // validation
        if (name.value && !value.value) {
            value.classList.add('invalid');
            return;
        }

        value.classList.remove('invalid');

        if (rows.length >= 10) return;

        const row = document.createElement('div');
        row.classList.add('form-row', 'mb-2');

        row.innerHTML = `
                <input type="text" placeholder="نام پارامتر" class="form-control param-name">
                <input type="text" placeholder="مقدار پارامتر" class="form-control param-value">
                <button class="remove-btn" onclick="removeRow(this)">✕</button>
            `;

        container.appendChild(row);
    }

    container.addEventListener('input', () => {

        const rows = container.querySelectorAll('.form-row');
        const lastRow = rows[rows.length - 1];

        const name = lastRow.querySelector('.param-name');
        const value = lastRow.querySelector('.param-value');

        if (name.value.trim() !== "" && value.value.trim() !== "") {
            addRow();
        }
    });
    function getAllParams() {

        const name = document.getElementById('nameSelect').value;
        const bigName = document.getElementById('bigName').value;
        const bigText = document.getElementById('bigText').value;

        const params = [];

        container.querySelectorAll('.form-row').forEach(row => {

            const paramName = row.querySelector('.param-name').value;
            const paramValue = row.querySelector('.param-value').value;

            if (paramName || paramValue) {
                params.push({ paramName, paramValue });
            }
        });

        return {
            name,
            params,
            description: {
                bigName,
                bigText
            }
        };
    }


    function submitForm() {

        const data = getAllParams();

        if (!data) {
            alert("بعضی مقادیر خالی هستند");
            return;
        }

        console.log(data);
        alert("ارسال شد ✔");
    }
}
function removeRow(btn) {
    const container = document.getElementById('parametersContainer');
    const count = container.querySelectorAll("input").length;
    console.log(count);
    if (count == 2) {
        console.log(count);
        return;
    }
    btn.parentElement.remove();
}




