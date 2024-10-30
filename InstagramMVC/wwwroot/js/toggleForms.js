document.addEventListener("DOMContentLoaded", function() {
    var loginTab = document.getElementById('loginTab');
    var registerTab = document.getElementById('registerTab');
    var loginForm = document.getElementById('loginForm');
    var registerForm = document.getElementById('registerForm');

    // Sjekk at elementene eksisterer før vi legger til event listeners
    if (loginTab && registerTab && loginForm && registerForm) {
        loginTab.addEventListener('click', function(event) {
            event.preventDefault();
            loginForm.style.display = 'block';          // Vis logg inn-skjema
            registerForm.style.display = 'none';        // Skjul registrer-skjema
            loginTab.classList.add('active');           // Legg til aktiv klasse på logg inn-fanen
            registerTab.classList.remove('active');     // Fjern aktiv klasse fra registrer-fanen
        });

        registerTab.addEventListener('click', function(event) {
            event.preventDefault();
            registerForm.style.display = 'block';       // Vis registrer-skjema
            loginForm.style.display = 'none';           // Skjul logg inn-skjema
            registerTab.classList.add('active');        // Legg til aktiv klasse på registrer-fanen
            loginTab.classList.remove('active');        // Fjern aktiv klasse fra logg inn-fanen
        });
    }
});
