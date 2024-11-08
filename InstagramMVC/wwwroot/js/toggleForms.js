document.addEventListener("DOMContentLoaded", function () {
  var loginTab = document.getElementById("loginTab");
  var createUserTab = document.getElementById("createUserTab");
  var loginForm = document.getElementById("loginForm");
  var createUserForm = document.getElementById("createUserForm");

  if (loginTab && createUserTab && loginForm && createUserForm) {
    loginTab.addEventListener("click", function (event) {
      event.preventDefault();
      loginForm.style.display = "block"; // Vis logg inn-skjemaet
      createUserForm.style.display = "none"; // Skjul registreringsskjemaet
      loginTab.classList.add("active");
      createUserTab.classList.remove("active");
    });

    createUserTab.addEventListener("click", function (event) {
      event.preventDefault();
      createUserForm.style.display = "block"; // Vis registreringsskjemaet
      loginForm.style.display = "none"; // Skjul logg inn-skjemaet
      createUserTab.classList.add("active");
      loginTab.classList.remove("active");
    });
  }
});
