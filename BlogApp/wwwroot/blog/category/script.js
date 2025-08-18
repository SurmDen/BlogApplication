let pagButtons = document.querySelectorAll('.pag-btn');

Array.from(pagButtons).forEach(p => {
    p.addEventListener('mousedown', () => {
        document.querySelector('.pag-input').value = p.value;
    });
});