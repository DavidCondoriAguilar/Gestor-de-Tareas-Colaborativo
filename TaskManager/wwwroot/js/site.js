// Espera a que el DOM esté cargado
document.addEventListener("DOMContentLoaded", function() {

    // Selecciona todos los <select> de estado
    const selectoresEstado = document.querySelectorAll(".select-estado-tarea");

    selectoresEstado.forEach(select => {
        select.addEventListener("change", function() {

            const tareaId = this.dataset.tareaid; 
            const nuevoEstado = this.value;

            // Usamos fetch para la llamada asíncrona 
            fetch(`/?handler=UpdateEstado&tareaId=${tareaId}&nuevoEstado=${nuevoEstado}`, {
                method: "POST",
                // Importante: Necesitamos el token de verificación de Razor Pages
                headers: {
                    "RequestVerificationToken": document.getElementsByName("__RequestVerificationToken")[0].value
                }
            })
            .then(response => response.json())
            .then(data => {
                console.log(data.message); // "Estado actualizado"
                
                // Actualizar visualmente la tarjeta sin recargar la página
                const taskCard = this.closest('.task-card');
                const statusSpan = taskCard.querySelector('.task-status');
                
                // Remover clases de estado anteriores
                taskCard.classList.remove('estado-Pendiente', 'estado-EnProgreso', 'estado-Completada');
                statusSpan.classList.remove('estado-Pendiente', 'estado-EnProgreso', 'estado-Completada');
                
                // Agregar nueva clase de estado
                taskCard.classList.add(`estado-${nuevoEstado}`);
                statusSpan.classList.add(`estado-${nuevoEstado}`);
                statusSpan.textContent = nuevoEstado;
                
                // Mostrar mensaje de éxito
                showNotification('Estado actualizado correctamente', 'success');
            })
            .catch(error => {
                console.error("Error al actualizar:", error);
                showNotification('Error al actualizar el estado', 'error');
                
                // Revertir el select al valor anterior en caso de error
                this.selectedIndex = 0; // o mantener el valor anterior
            });
        });
    });
});

// Función para mostrar notificaciones
function showNotification(message, type) {
    // Crear elemento de notificación
    const notification = document.createElement('div');
    notification.className = `notification ${type}`;
    notification.textContent = message;
    
    // Estilos inline para la notificación
    notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        padding: 1rem 1.5rem;
        border-radius: 8px;
        color: white;
        font-weight: 600;
        z-index: 1000;
        transform: translateX(100%);
        transition: transform 0.3s ease;
        ${type === 'success' ? 'background-color: #27ae60;' : 'background-color: #e74c3c;'}
    `;
    
    // Agregar al DOM
    document.body.appendChild(notification);
    
    // Animar entrada
    setTimeout(() => {
        notification.style.transform = 'translateX(0)';
    }, 100);
    
    // Remover después de 3 segundos
    setTimeout(() => {
        notification.style.transform = 'translateX(100%)';
        setTimeout(() => {
            document.body.removeChild(notification);
        }, 300);
    }, 3000);
}
