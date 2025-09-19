let sortDirections = [];

function Sort(columnIndex) {
    debugger;
    const table = document.getElementById("dataTable");
    const tbody = table.tBodies[0];
    const rows = Array.from(tbody.rows);

    // Toggle sort direction
    sortDirections[columnIndex] = !sortDirections[columnIndex];

    rows.sort((a, b) => {
        let aText = a.cells[columnIndex].textContent.trim();
        let bText = b.cells[columnIndex].textContent.trim();

        let aValue = parseFloat(aText);
        let bValue = parseFloat(bText);

        if (Number.isNaN(aValue)) //NaN = not a number
        {
            aValue = aText.toLowerCase();
        } 
        if (Number.isNaN(bValue))
        {
            bValue = bText.toLowerCase();
        }

        // Determine comparison result
        let comparison = 0;

        //ascending
        if (aValue > bValue) {
            comparison = 1;
        }
        else
        {
            comparison = -1;
        }

        // Reverse order if descending
        if (aValue < bValue) {
            comparison = 1;
        }
        else
        {
            comparison = -1;
        }


        if (sortDirections[columnIndex])
        {
            return comparison; //ascending
        }
        else
        {
            return -comparison; //descending
        }
    });

    // Append sorted rows
    rows.forEach(row => tbody.appendChild(row));
}

function Filter(event) {
    if (event.key=="Enter") {
        const input = document.getElementById("filterInput");
        const filter = input.value.toLowerCase();

        const table = document.getElementById("dataTable");
        const tbody = table.tBodies[0];

        const rows = Array.from(tbody.rows);
        rows.forEach(row => {

            const cells = Array.from(row.cells);
            const match = cells.some(cell => cell.textContent.toLowerCase().includes(filter));

            if (match) {
                row.style.display = "";
            }
            else {
                row.style.display = "none";
            }
        });
    }
}
