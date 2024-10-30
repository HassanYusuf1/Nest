document.addEventListener("DOMContentLoaded", function() {
    var loginTab = document.getElementById('loginTab');
    var registerTab = document.getElementById('registerTab');
    var loginForm = document.getElementById('loginForm');
    var registerForm = document.getElementById('registerForm');

    if (loginTab && registerTab && loginForm && registerForm) {
        loginTab.addEventListener('click', function(event) {
            event.preventDefault();
            loginForm.style.display = 'block'; // Vis logg inn-skjemaet
            registerForm.style.display = 'none'; // Skjul registreringsskjemaet
            loginTab.classList.add('active');
            registerTab.classList.remove('active');
        });

        registerTab.addEventListener('click', function(event) {
            event.preventDefault();
            registerForm.style.display = 'block'; // Vis registreringsskjemaet
            loginForm.style.display = 'none'; // Skjul logg inn-skjemaet
            registerTab.classList.add('active');
            loginTab.classList.remove('active');
        });
    }
});
