// Add this to wwwroot/app.js or a new JS file and reference it in index.html or _Host.cshtml
// Requires html2pdf.js (https://github.com/eKoopmans/html2pdf)
// You can add html2pdf via CDN in your index.html/_Host.cshtml:
// <script src="https://cdnjs.cloudflare.com/ajax/libs/html2pdf.js/0.10.1/html2pdf.bundle.min.js"></script>

window.attfcExportPdf = function (elementId) {
    var element = document.getElementById(elementId);
    if (!element) return;
    var opt = {
        margin:       0.5,
        filename:     'AttfcReport.pdf',
        image:        { type: 'jpeg', quality: 0.98 },
        html2canvas:  { scale: 2 },
        jsPDF:        { unit: 'in', format: 'letter', orientation: 'portrait' },
    };
    html2pdf().set(opt).from(element).save();
}
