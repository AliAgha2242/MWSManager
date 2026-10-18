let arrows = document.querySelectorAll(".arrow");
arrows.forEach((arrow) => {
    arrow.addEventListener("click", (e) => {
        let arrowParent = arrow.closest("li");
        arrowParent.classList.toggle("showMenu");
        console.log("hello")
    });
});
let sidebar = document.querySelector(".sidebar");
let sidebarBtn = document.querySelector(".bx-menu");
console.log(sidebarBtn);
sidebarBtn.addEventListener("click", () => {
    sidebar.classList.toggle("close");
});