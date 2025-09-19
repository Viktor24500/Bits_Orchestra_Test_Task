let sortDirections = [];

function Sort(columnIndex) {
    const table = document.getElementById("dataTable");
    const tbody = table.tBodies[0];
    const rows = Array.from(tbody.rows);

    // Toggle sort direction
    sortDirections[columnIndex] = !sortDirections[columnIndex];

    rows.sort((a, b) => {
        let aText = a.cells[columnIndex].textContent.trim();
        let bText = b.cells[columnIndex].textContent.trim();

        // Try to convert to numbers, fallback to string
        let aValue = isNaN(aText) ? aText.toLowerCase() : parseFloat(aText);
        let bValue = isNaN(bText) ? bText.toLowerCase() : parseFloat(bText);

        if (aValue > bValue) return sortDirections[columnIndex] ? 1 : -1;
        if (aValue < bValue) return sortDirections[columnIndex] ? -1 : 1;
        return 0;
    });

    // Append sorted rows
    rows.forEach(row => tbody.appendChild(row));
}