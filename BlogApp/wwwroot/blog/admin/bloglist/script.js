let removeButtons = document.querySelectorAll('.remove-btn');
let removeInput = document.querySelector('.remove-input');
let errorBox = document.querySelector('.error-box');
let closeButton = document.querySelector('.error-box-close-btn');

Array.from(removeButtons).forEach(rb => {
    rb.addEventListener('click', () => {
        let aliasValue = rb.getAttribute('value');

        removeInput.value = aliasValue;

        errorBox.style.display = 'flex';
    });
});

closeButton.addEventListener('click', () => {
    removeInput.value = '';

    errorBox.style.display = 'none';
});

let deleteButtons = document.querySelectorAll('.remove-forever-btn');

Array.from(deleteButtons).forEach(b => {
    b.addEventListener('click', async (e) => {
        const alias = b.getAttribute('value');

        if (alias !== '') {
            const answer = await fetch(`/blog/api/delete/forever/${alias}`);

            if (answer.ok) {
                e.target.closest('.blog-element').remove();
            }
        }
    });
});


