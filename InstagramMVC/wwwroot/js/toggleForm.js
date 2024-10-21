document.addEventListener("DOMContentLoaded", function() {
    var loginTab = document.getElementById('loginTab');
    var registerTab = document.getElementById('registerTab');
    var loginForm = document.getElementById('loginForm');
    var registerForm = document.getElementById('registerForm');

    // Sjekk at elementene eksisterer før du legger til event listeners
    if (loginTab && registerTab && loginForm && registerForm) {
        loginTab.addEventListener('click', function() {
            loginForm.classList.add('active');
            registerForm.classList.remove('active');
            this.classList.add('active');
            registerTab.classList.remove('active');
        });

        registerTab.addEventListener('click', function() {
            registerForm.classList.add('active');
            loginForm.classList.remove('active');
            this.classList.add('active');
            loginTab.classList.remove('active');
        });
    }
});
