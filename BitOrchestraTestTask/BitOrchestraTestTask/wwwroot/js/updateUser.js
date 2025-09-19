//function EditUser(index)
//{
//    debugger;
//    let userId = document.getElementsByClassName("UserIdInTable")[index].textContent.trim();
//    let userName = document.getElementsByClassName("UserNameInTable")[index].textContent.trim();
//    let dateOfBirth = document.getElementsByClassName("DateOfBirthInTable")[index].textContent.trim();
//    let married = document.getElementsByClassName("MarriedInTable")[index].textContent.trim();
//    let phone = document.getElementsByClassName("PhoneInTable")[index].textContent.trim();
//    let salary = document.getElementsByClassName("SalaryInTable")[index].textContent.trim();

//    document.getElementById("Id").value = userId;
//    document.getElementById("Name").value = userName;
//    document.getElementById("DateOfBirth").value = dateOfBirth;

//    document.getElementById("Married").checked = (married.toLowerCase() === "true");

//    document.getElementById("Phone").value = phone;
//    document.getElementById("Salary").value = salary;

//    document.getElementsByClassName("formClass")[0].style.display = "block";
//}

function EditUser(button) {
    const row = button.closest('tr');
    let field;
    let value;
    row.querySelectorAll('.editable').forEach(cell => {
        field = cell.getAttribute('data-field');
        value = cell.textContent.trim();
        cell.innerHTML = '<input type="text" name=' + field + ' value= ' + value +' />';
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

    inputs.forEach(input => {
        debugger;
        if (input.name == 'Married') {
            data[input.name] = input.value.toLowerCase()==="true";
        }
        else {
            data[input.name] = input.value;
}
        
        });

/*    debugger;*/
    let Http = new XMLHttpRequest();
    let url = `/Home/UpdateUser`
    Http.open("POST", url);
    Http.setRequestHeader("Content-Type", "application/json; charset=utf-8")
    Http.send(JSON.stringify(data));

    Http.onreadystatechange = function () {
        if (Http.readyState === 4 && Http.status === 200) {
            console.log(Http.responseText);
            if (Http.status === 200) {
                window.location.href = '/user';
            }
        }
    };
}