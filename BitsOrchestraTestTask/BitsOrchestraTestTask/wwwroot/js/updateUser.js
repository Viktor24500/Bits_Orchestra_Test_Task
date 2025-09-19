function EditUser(button) {
    const row = button.closest('tr');
    let field;
    let value;
    row.querySelectorAll('.editable').forEach(cell => {
        field = cell.getAttribute('data-field');
        value = cell.textContent.trim();
        switch (field)
        {
            case 'DateOfBirth':
                cell.innerHTML = `<input type="date" name="${field}" value="${value}" />`;
                break;
            case 'Married':
                const isChecked = value.toLowerCase() === "true";
                if (isChecked) {
                    cell.innerHTML = `<input type="checkbox" name="${field}" isChecked='checked' checked/>`;
                }
                else
                {
                    cell.innerHTML = `<input type="checkbox" name="${field}" isChecked='' />`;
                }
                break;
            case 'Salary':
                cell.innerHTML = `<input type="number" min=0 step=0.01 name="${field}" value="${value}" />`;
                break;
            default:
                cell.innerHTML = `<input type="text" name="${field}" value="${value}"/>`;
                break;
        }

    });

    // Toggle buttons
    row.querySelector('.editBtn').style.display = 'none';
    row.querySelector('.saveBtn').style.display = 'block';
}

function SaveUser(button) {
    const row = button.closest('tr');
    const id = row.getAttribute('data-id');
    const inputs = row.querySelectorAll('input');
    const data = { Id: id };
    const marriedCheckbox = row.querySelector('input[name="Married"]');

    inputs.forEach(input => {
        data.Married = marriedCheckbox.checked;
        data[input.name] = input.value;
    });

    let Http = new XMLHttpRequest();
    let url = `/Home/UpdateUser`
    Http.open("POST", url);
    Http.setRequestHeader("Content-Type", "application/json; charset=utf-8")
    Http.send(JSON.stringify(data));

    Http.onreadystatechange = function () {
        if (Http.readyState === 4) {
            if (Http.status === 200) {
                window.location.href = '/Home/GetAllUsers';
                row.querySelector('.editBtn').style.display = 'block';
                row.querySelector('.saveBtn').style.display = 'none';
            }
            else
            {
                console.log(Http.responseText);
                window.location.href = '/Home/GetErrorPage';
            }
        }
    };
}